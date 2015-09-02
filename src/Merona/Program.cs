using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using MongoDB.Driver;
using MongoDB.Bson;

namespace Merona
{
    using TestApp;

    class Program
    {
        static void Main(string[] args)
        {
            // TODO : config
            NLog.Config.SimpleConfigurator.ConfigureForConsoleLogging(NLog.LogLevel.Debug);
            
           // var r = DataBinder.Bind("#{bar.foo} sadf dee #{rre}", new Session());
           // Console.WriteLine(r);

            var aa = new List<int>() { 1, 2, 3, 4 };
            var bb = new List<int>() { 1, 2, 3, 4 };

            Console.WriteLine(aa.Equals(bb));

            var config = Config.defaults;

            config.clusterPeers.Add(new Tuple<String, int>("localhost", 9915));

            FooPacket f = new FooPacket();
            TestService a = new TestService();
            Server s = new Server(config);

            s.AttachService<TestService>();
            s.Start();

            f.pid = "asdf";

            var bp = new BarPacket();
            bp.resp = "QWER";
            s.Enqueue(new Server.RecvPacketEvent(new Session(), bp));

            s.AddPreProcessor(delegate (Session session, Packet packet)             {
                var bar = (BarPacket)packet;

                Console.WriteLine("prep1");

                bar.resp = "wwQQ";
            }, 1);
            s.AddPreProcessor(delegate(Session session, Packet packet)
            {
                var bar = (BarPacket)packet;

                Console.WriteLine("prep2");

                bar.resp = "wwQQ";
            }, 0);

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                s.Kill();
            };

            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
