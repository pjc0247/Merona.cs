using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public sealed partial class Server
    {
        public interface IMarshalContext{
            /// <summary>
            /// 이 메소드를 상속하여 커스텀 Marshal.Serialize를 구현한다.
            /// 이 메소드는 null을 리턴할 때까지 반복 실행된다.
            /// </summary>
            /// <param name="buffer">Serialize해야 할 패킷의 목록</param>
            /// <returns>
            /// 직렬화 된 바이트 배열,
            /// 만약 더 이상 직렬화 할 수 없거나, 남은 패킷이 없으면 null을 리턴해야 함
            /// </returns>
            byte[] Serialize(CircularBuffer<Packet> buffer);

            /// <summary>
            /// 이 메소드를 상속하여 커스텀 Marshal.Deserialize를 구현한다.
            /// 이 메소드는 null을 리턴할 때까지 반복 실행된다.
            /// </summary>
            /// <param name="buffer">Deserialize해야 할 남아있는 바이트들의 목록</param>
            /// <returns>
            /// 바이트로부터 역직렬화 된 단일 패킷
            /// 만약 더 이상 역직렬화 할 수 없거나, 남은 패킷이 없으면 null을 리턴해야 함
            /// </returns>
            Packet Deserialize(CircularBuffer<byte> buffer);
        }

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
