using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

using Merona.Client;

namespace ChattingClient
{
    class Program
    {
        static TcpClient client;

        static byte[] BlockingRecv(int size)
        {
            var buffer = new byte[size];
            var recved = 0;

            while(recved < size)
            {
                recved += client.Client.Receive(buffer, recved, size - recved, SocketFlags.None);
            }

            return buffer;
        }

        static void RecvThread()
        {
            while (true)
            {
                var bSize = BlockingRecv(4);
                var bPacketId = BlockingRecv(4);
                var size = BitConverter.ToInt32(bSize, 0);
                var packetId = BitConverter.ToInt32(bPacketId, 0);

                Console.WriteLine(size);
                Console.WriteLine(packetId);

                var bPacket = BlockingRecv(size - 8);
                var bCompletePacket = new byte[size];

                bSize.CopyTo(bCompletePacket, 0);
                bPacketId.CopyTo(bCompletePacket, 4);
                bPacket.CopyTo(bCompletePacket, 8);

                var type = Packet.GetTypeById(packetId);
                var packet = Packet.Deserialize(bCompletePacket, type);

                if(packet.GetType() == typeof(Packets.ChatMessage.S2C))
                {
                    var chatPacket = (Packets.ChatMessage.S2C)packet;

                    Console.WriteLine(chatPacket.message);
                }
            }
        }

        static void Main(string[] args)
        {
            client = new TcpClient();

            try {
                client.Connect("localhost", 9916);

                var thread = new Thread(RecvThread);
                thread.Start();

                var join = new Packets.Join.C2S();
                join.nickname = "hello";
                client.Client.Send(join.Serialize());

                while (true)
                {
                    var message = Console.In.ReadLine();
                    var packet = new Packets.ChatMessage.C2S();

                    packet.message = "SADF";

                    client.Client.Send(packet.Serialize());
                }
            }
            catch(Exception e)
            {

            }
        }
    }
}
