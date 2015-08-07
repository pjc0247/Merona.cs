using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Merona.TestApp
{
    class Player : Model
    {
        public string name;
    }

    [PacketId(0)]
    [AutoResponse(typeof(BarPacket))]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class FooPacket : Packet
    {
        [KeyOf(typeof(Player))]
        public string pid;

        [MemberOf(typeof(Player))]
        public int vv;

        [Bind("#{bar.foo}")]
        public string bind;
    }
    [PacketId(1)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class BarPacket : Packet
    {
        //[Bind("bind")]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
        public String resp;
    }

    class TestService : Service
    {
        [Handler(typeof(BarPacket))]
        public async void OnBar(Session session, BarPacket packet)
        {
            Console.WriteLine("OnBar ");

            await Scheduler.current.Yield(2000);
            Console.WriteLine("NExt");
            //Console.WriteLine(packet.data);
        }
        [Handler(typeof(FooPacket))]
        public async void OnFoo(Session session, FooPacket packet)
        {
            FooPacket f = new FooPacket();
            f.Serialize();

            Console.WriteLine("OnPacket " + Thread.CurrentThread.ManagedThreadId.ToString());
            packet.Dump();


            var a = Scheduler.current.Schedule(() =>
            {
                Console.WriteLine("hello world");
            }, 100, 0, 2);
            a.Cancel();
            //

            Model.FindOneAsync<Player>(packet);
        }

        [Handler("world.chat")]
        public async void OnChat(Session session, FooPacket packet)
        {
            Console.WriteLine("onChat");
        }
    }
}
