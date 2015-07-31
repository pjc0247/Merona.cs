using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Merona
{
    public partial class Session
    {
        [ThreadStatic]
        public static Session current = null;

        public String test { get; set; }

        public Session()
        {
            test = "ASDF";
            bar = new Bar();

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
                int size = 0;

                buffer.Put(receiveBuffer, 0, received);

                //BitConverter.ToInt32()
                //Server.current.Enqueue(this, null);
            }
            catch (InternalBufferOverflowException e)
            {
                Console.WriteLine("ringbuffer overflow");
            }
            finally
            {
                BeginReceive();
            }
        }

    }
}
