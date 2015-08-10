using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Merona;

namespace ChattingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Config.defaults;
            config.sessionType = typeof(MySession);

            var server = new Server(config);
            server.AttachService<ChatService>();
            server.Start();

            while (true)
            {
                Console.WriteLine("running...");
                Thread.Sleep(1000);
            }
        }
    }
}
