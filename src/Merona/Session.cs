using System;
using System.IO;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Merona
{
    public partial class Session : IStatusObservable<Session>
    {
        /// <summary>
        /// 현재 바인딩 된 세션의 인스턴스
        /// 이 값은 Worker 스레드에서만 유효하다.
        /// </summary>
        [ThreadStatic]
        public static Session current = null;

        public Server server { get; protected set; }
        public bool isAlive { get; private set; }
        public TcpClient client { get; private set; }

        public HashSet<Channel> channels { get; private set; }

        internal IMarshalContext marshaler { get; private set; }

        private byte[] receiveBuffer { get; set; }
        internal CircularBuffer<byte> receiveRingBuffer { get; set; }
        internal CircularBuffer<byte> sendRingBuffer { get; set; }
        internal CircularBuffer<Packet> pendingPackets { get; set; }
        internal long skip;

        public Session()
        {
            this.channels = new HashSet<Channel>();
            this.receiveRingBuffer = new CircularBuffer<byte>(1024); /* TODO : config */
            this.sendRingBuffer = new CircularBuffer<byte>(1024); /* TODO : config */
            this.pendingPackets = new CircularBuffer<Packet>(1024);
            this.receiveBuffer = new byte[128]; /* TODO : config */
            this.marshaler = (IMarshalContext)Activator.CreateInstance(Config.defaults.marshalerType);
            this.skip = 0;
            this.pipelineContext = new PipelineContext();

            InitializeSafeCollectionSupport();
        }
        public Session(Server server)
        {
            this.server = server;
            this.channels = new HashSet<Channel>();
            this.receiveRingBuffer = new CircularBuffer<byte>(server.config.sessionRingBufferSize);
            this.sendRingBuffer = new CircularBuffer<byte>(server.config.sessionRingBufferSize);
            this.pendingPackets = new CircularBuffer<Packet>(server.config.sessionRingBufferSize);
            this.receiveBuffer = new byte[server.config.sessionRecvBufferSize];
            this.marshaler = (IMarshalContext)Activator.CreateInstance(server.config.marshalerType);
            this.skip = 0;
            this.pipelineContext = new PipelineContext();

            InitializeSafeCollectionSupport();
        }

        internal protected virtual void OnConnect()
        {
            Server.current.logger.Info("Session OnConnect");
        }
        internal protected virtual void OnDisconnect()
        {
            Server.current.logger.Info("Session OnDisconnect");
        }

        internal void Reset(TcpClient client)
        {
            PublishInvalidated();

            this.client = client;
            this.isAlive = true;
            this.marshaler =
                (IMarshalContext)Activator.CreateInstance(server.config.marshalerType);

            foreach (var channel in channels)
                channel.Leave(this);

            BeginReceive();
        }

        /// <summary>
        /// 현재 연결을 끊고, 세션을 정리한다.
        /// [Non-Thread-Safe]
        /// </summary>
        public void Disconnect()
        {
            PublishInvalidated();
            isAlive = false;

            if (client != null &&
                client.Client.Connected) {
                client.Client.BeginDisconnect(
                    true,
                    new AsyncCallback(Disconneted), null);
            }
        }
        private void Disconneted(IAsyncResult result)
        {
            try
            {
                client.Client.EndDisconnect(result);  
            }
            catch(Exception e)
            {
                server.logger.Error("Session::Disconnected", e);
                server.watcher.OnServerException(e);
            }
            finally
            {
                // 현재 세션을 다시 사용 가능하도록 만든다.
                server.sessionPool.Return(this);
            }
        }

        /// <summary>
        /// 패킷 전송을 요청한다.
        /// 이 메소드는 패킷에 후처리를 수행한 후,
        /// IO 쓰레드의 전송 대기 패킷 목록에 패킷을 담은 후
        /// 반환하며, 실제 전송은 IO 쓰레드에서 수행된다.
        /// [Non-Thread-Safe]
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public int Send(Packet packet)
        {
            if (!isAlive)
                return 0;

            packet.PostProcess(this);

            pendingPackets.Put(packet);
            server.ioWorker.Pulse(this);

            return 0;
        }

        /// <summary>
        /// 링버퍼에 쌓인 데이터를 전송 시도한다.
        /// 이 메소드는 IoWorker에 의해서 실행된다.
        /// </summary>
        /// <param name="size">전송할 길이, -1일 경우 전부 전송</param>
        internal void FlushSend(int size = -1)
        {
            if(size == -1)
                size = sendRingBuffer.Size;

            try {
                var bufferToSend = new byte[size];

                sendRingBuffer.Peek(bufferToSend, 0, size);

                client.Client.BeginSend(
                    bufferToSend, 0, size, SocketFlags.None,
                    new AsyncCallback(Sent), size);
            }
            catch(Exception e)
            {
                server.logger.Error("Session::FlushSend", e);
                server.watcher.OnServerException(e);

                Disconnect();
            }
        }
        private void Sent(IAsyncResult result)
        {
            try
            {
                int sent = client.Client.EndSend(result);

                Interlocked.Add(ref skip, sent);

                // 요청양보다 실제 전송된 양이 작으면
                // 재전송 요청
                // TODO : 카운팅
                if (sendRingBuffer.Size > (int)result.AsyncState)
                    server.ioWorker.Pulse(this);
            }
            catch(Exception e)
            {
                server.logger.Error("Session::Sent", e);
                server.watcher.OnServerException(e);

                Disconnect();
            }
        }

        protected void BeginReceive()
        {
            try {
                this.client.Client.BeginReceive(
                    receiveBuffer, 0, receiveBuffer.Length,
                    SocketFlags.None,
                    new AsyncCallback(Received), null);
            }
            catch(SocketException e)
            {
                server.logger.Error("Session::BeginReceive", e);
                server.watcher.OnServerException(e);

                Disconnect();
            }
        }
        private void Received(IAsyncResult result)
        {
            try
            {
                var received = client.Client.EndReceive(result);

                if(received <= 0)
                    throw new ObjectDisposedException("socket closed");
                
                receiveRingBuffer.Put(receiveBuffer, 0, received);

                while (true)
                {
                    var packet =
                        marshaler.Deserialize(receiveRingBuffer);

                    if (packet == null)
                        break;

                    server.Enqueue(new Server.RecvPacketEvent(this, packet));   
                }

                BeginReceive();
            }
            catch (Exception e)
            {
                server.logger.Error("Server::Received", e);
                server.watcher.OnServerException(e);

                Disconnect();
            }
        }

    }
}
