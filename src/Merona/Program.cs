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
            var r = DataBinder.Bind("#{bar.foo} sadf dee #{rre}", new Session());
            Console.WriteLine(r);

            var aa = new List<int>() { 1, 2, 3, 4 };
            var bb = new List<int>() { 1, 2, 3, 4 };

            Console.WriteLine(aa.Equals(bb));

            FooPacket f = new FooPacket();
            TestService a = new TestService();
            Server s = new Server();

            s.AttachService(a);
            s.Start();

            f.pid = "asdf";

            s.Enqueue(new Session(), f);
            s.Enqueue(new Session(), f);

            while (true)
            {
                

                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
