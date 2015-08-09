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
    public partial class Session
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

        internal Server.IMarshalContext marshaler { get; private set; }

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
            this.marshaler = (Server.IMarshalContext)Activator.CreateInstance(Config.defaults.marshalerType);
            this.skip = 0;
        }
        public Session(Server server)
        {
            this.server = server;
            this.channels = new HashSet<Channel>();
            this.receiveRingBuffer = new CircularBuffer<byte>(server.config.sessionRingBufferSize);
            this.sendRingBuffer = new CircularBuffer<byte>(server.config.sessionRingBufferSize);
            this.pendingPackets = new CircularBuffer<Packet>(server.config.sessionRingBufferSize);
            this.receiveBuffer = new byte[server.config.sessionRecvBufferSize];
            this.marshaler = (Server.IMarshalContext)Activator.CreateInstance(server.config.marshalerType);
            this.skip = 0;
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
            this.client = client;
            this.isAlive = true;
            this.marshaler =
                (Server.IMarshalContext)Activator.CreateInstance(server.config.marshalerType);

            foreach (var channel in channels)
                channel.Leave(this);

            BeginReceive();
        }
        

        public void Disconnect()
        {
            isAlive = false;

            if (client.Client.Connected) {
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
                server.logger.Warn("Session::Disconnected", e);
            }
            finally
            {
                // 현재 세션을 다시 사용 가능하도록 만든다.
                server.sessionPool.Return(this);
            }
        }

        public int Send(Packet packet)
        {
            if (!isAlive)
                return 0;

            packet.PostProcess(this);

            pendingPackets.Put(packet);
            server.ioWorker.Pulse(this);

            return 0;
        }
        internal void FlushSend(int length = -1)
        {
            if(length == -1)
                length = sendRingBuffer.Size;
            var bufferToSend = new byte[length];

            sendRingBuffer.Peek(bufferToSend, 0, length);
            client.Client.BeginSend(
                bufferToSend, 0, length, SocketFlags.None,
                new AsyncCallback(Sent), length);
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
                server.logger.Warn("Session::Sent", e);

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
                server.logger.Warn("Session::BeginReceive", e);

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
                server.logger.Warn("Server::Received", e);

                Disconnect();
            }
        }

    }
}
