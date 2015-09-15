using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona.Go
{
    public class GoPacket : Packet
    {
        public int objectId;
    }

    public class SyncProperty
    {
        public class Request : GoPacket
        {
            public String key;
            public object value;
        }
        public class Response : GoPacket
        {
        }
    }
    public class RpcCall
    {
        public class Request : Packet
        {
            public String method;
            public object[] args;
            public int objectId;
        }
        public class Response : Packet
        {
            public object response;
            public Exception e;
            public int objectId;
        }
    }
}
