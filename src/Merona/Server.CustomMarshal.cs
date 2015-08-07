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
            internal protected abstract byte[] Serialize(CircularBuffer<Packet> buffer);
            internal protected abstract Packet Deserialize(CircularBuffer<byte> buffer);
        }

        private class DefaultMarshaler : Marshaler
        {
            internal protected override byte[] Serialize(CircularBuffer<Packet> buffer)
            {
                if (buffer.Size == 0)
                    return null;

                return buffer.Get().Serialize();
            }
            internal protected override Packet Deserialize(CircularBuffer<byte> buffer)
            {
                var bytes = new byte[Packet.headerSize];

                if (buffer.Size >= Packet.headerSize)
                {
                    buffer.Peek(bytes, 0, bytes.Length);
                    int size = BitConverter.ToInt32(bytes, 0);
                    int packetId = BitConverter.ToInt32(bytes, 4);

                    if (buffer.Size >= size)
                    {
                        var packet = new byte[size];

                        buffer.Get(packet, 0, packet.Length);

                        var packetType = Packet.GetTypeById(packetId);
                        var deserialized = Packet.Deserialize(packet, packetType);

                        return deserialized;
                    }
                }

                return null;
            }
        }

        public Marshaler marshaler { get; set; }

        void InitializeMarshaler()
        {
            marshaler = new DefaultMarshaler();
        }
    }
}
