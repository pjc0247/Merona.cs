using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public sealed partial class Server
    {
        /// <summary>
        /// 기본적으로 제공되는 마샬러,
        /// 단순 바이트 배열간의 직렬화, 역직렬화 기능을 제공한다.
        /// </summary>
        internal class DefaultMarshaler : IMarshalContext
        {
            public byte[] Serialize(CircularBuffer<Packet> buffer)
            {
                if (buffer.Size == 0)
                    return null;

                return buffer.Get().Serialize();
            }
            public Packet Deserialize(CircularBuffer<byte> buffer)
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
    }
}
