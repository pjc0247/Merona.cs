using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace Merona.TestApp
{
    class Player : Model
    {
        public string name;
    }
    class FooPacket : Packet
    {
        [KeyOf(typeof(Player))]
        public string pid;

        [MemberOf(typeof(Player))]
        public int vv;

        [Bind("#{bar.foo}")]
        public string bind;
    }
    class BarPacket : Packet
    {
    }

    class TestService : Service
    {
        [Handler(typeof(FooPacket))]
        public async void OnFoo(FooPacket packet)
        {
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
        public async void OnChat(FooPacket packet)
        {
            Console.WriteLine("onChat");
        }
    }
}
