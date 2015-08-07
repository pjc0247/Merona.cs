using System;
using System.IO;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Server server { get; private set; }
        public bool isAlive { get; private set; }
        public TcpClient client { get; private set; }

        public HashSet<Channel> channels { get; private set; }

        private byte[] receiveBuffer { get; set; }
        internal CircularBuffer<byte> receiveRingBuffer { get; set; }
        internal CircularBuffer<byte> sendRingBuffer { get; set; }
        internal CircularBuffer<Packet> pendingPackets { get; set; }

        public Session()
        {
            this.channels = new HashSet<Channel>();
            this.receiveRingBuffer = new CircularBuffer<byte>(1024); /* TODO : config */
            this.sendRingBuffer = new CircularBuffer<byte>(1024); /* TODO : config */
            this.pendingPackets = new CircularBuffer<Packet>(1024);
            this.receiveBuffer = new byte[128]; /* TODO : config */
        }
        public Session(Server server)
        {
            this.server = server;
            this.channels = new HashSet<Channel>();
            this.receiveRingBuffer = new CircularBuffer<byte>(server.config.sessionRingBufferSize);
            this.sendRingBuffer = new CircularBuffer<byte>(server.config.sessionRingBufferSize);
            this.pendingPackets = new CircularBuffer<Packet>(server.config.sessionRingBufferSize);
            this.receiveBuffer = new byte[server.config.sessionRecvBufferSize];
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

            foreach (var channel in channels)
                channel.Leave(this);

            BeginReceive();
        }

        public int Send(Packet packet)
        {
            if (!isAlive)
                return 0;

            pendingPackets.Put(packet);
            server.ioWorker.Pulse(this);

            return 0;
        }
        internal void Sent(IAsyncResult result)
        {
            try
            {
                int sent = client.Client.EndSend(result);

                sendRingBuffer.Skip(sent);

                // 보냈는데도, 링버퍼에 데이터가 남아있으면 -> 전송 실패
                // 재전송 요청
                // TODO : 카운팅
                if(sendRingBuffer.Size > 0)
                    server.ioWorker.Pulse(this);
            }
            catch(Exception e)
            {
                server.logger.Warn("Session::Sent", e);
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

                isAlive = false;
            }
        }
        internal void Received(IAsyncResult result)
        {
            try
            {
                var received = client.Client.EndReceive(result);

                if(received <= 0)
                    throw new ObjectDisposedException("socket closed");

                var bytes = new byte[Packet.headerSize];

                receiveRingBuffer.Put(receiveBuffer, 0, received);

                while(receiveRingBuffer.Size >= Packet.headerSize)
                { 
                    receiveRingBuffer.Peek(bytes, 0, bytes.Length);
                    int size = BitConverter.ToInt32(bytes, 0);
                    int packetId = BitConverter.ToInt32(bytes, 4);

                    if(receiveRingBuffer.Size >= size)
                    {
                        var packet = new byte[size];

                        receiveRingBuffer.Get(packet, 0, packet.Length);
                        
                        var packetType = Packet.GetTypeById(packetId);
                        var deserialized = Packet.Deserialize(packet, packetType);

                        server.Enqueue(new Server.RecvPacketEvent(this, deserialized));
                    }
                    else
                        break;
                }

                BeginReceive();
            }
            catch (InternalBufferOverflowException e)
            {
                server.logger.Warn("ringbuffer overflow");

                isAlive = false;
            }
            catch (SocketException e)
            {
                server.logger.Warn("Server::Receive", e);

                isAlive = false;
            }
        }

    }
}
