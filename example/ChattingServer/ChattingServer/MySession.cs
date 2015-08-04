using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Merona;

namespace ChattingServer
{
    class MySession : Session
    {
        public String nickname { get; set; }

        public MySession(Server server)
        {
            this.server = server;
        }
    }
}
