using System;
using System.IO;
using System.Collections;
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

        public Server server { get; set; }
        public String test { get; set; }

        public Session()
        {
            test = "ASDF";
            bar = new Bar();

            this.buffer = new CircularBuffer<byte>(1024); /* TODO : config */
            this.receiveBuffer = new byte[128]; /* TODO : config */
        }
        public Session(Server server)
        {
            test = "ASDF";
            bar = new Bar();
            
            this.server = server;
            this.buffer = new CircularBuffer<byte>(1024); /* TODO : config */
            this.receiveBuffer = new byte[128]; /* TODO : config */
        }

        public class Bar
        {
            public String foo { get; set; }
            public Bar()
            {
                foo = "foo";
            }
        }
        public Bar bar { get; set; }

        public String Bind(String format)
        {
            return "";
        }

        public TcpClient client { get; private set; }

        private byte[] receiveBuffer { get; set; }
        private CircularBuffer<byte> buffer { get; set; }

        internal protected virtual void OnConnect()
        {
            Server.current.logger.Info("Session OnConnect");
        }
        internal protected virtual void OnDisconnect()
        {
            Server.current.logger.Info("Session OnDisconnect");
        }

        public void Reset(TcpClient client)
        {
            this.client = client;

            BeginReceive();
        }

        protected void BeginReceive()
        {
            this.client.Client.BeginReceive(
                receiveBuffer, 0, receiveBuffer.Length,
                SocketFlags.None,
                new AsyncCallback(Receive), null);
        }
        private void Receive(IAsyncResult result)
        {
            try
            {
                var received = client.Client.EndReceive(result);
                var bytes = new byte[Packet.headerSize];

                buffer.Put(receiveBuffer, 0, received);

                while(buffer.Size >= Packet.headerSize)
                { 
                    buffer.Peek(bytes, 0, bytes.Length);
                    int size = BitConverter.ToInt32(bytes, 0);
                    int packetId = BitConverter.ToInt32(bytes, 4);

                    if(buffer.Size >= size)
                    {
                        var packet = new byte[size];

                        buffer.Get(packet, 0, packet.Length);
                        
                        var packetType = Packet.GetTypeById(packetId);
                        var deserialized = Packet.Deserialize(packet, packetType);
                        
                        server.Enqueue(this, deserialized);
                    }
                    else
                        break;
                }
            }
            catch (InternalBufferOverflowException e)
            {
                server.logger.Warn("ringbuffer overflow");
            }
            finally
            {
                BeginReceive();
            }
        }

    }
}
