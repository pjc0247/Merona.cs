using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public sealed partial class Server
    {
        public abstract class Marshaler{
            internal protected abstract void Serialize(CircularBuffer<byte> buffer);
            internal protected abstract void Deserialize(CircularBuffer<Packet> buffer);
        }
    }
}
