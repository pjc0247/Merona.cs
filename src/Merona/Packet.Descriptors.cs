using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    [AttributeUsage(AttributeTargets.Class)]
    class PacketId : Attribute
    {
        public int id { get; set; }
        public PacketId(int id)
        {
            this.id = id;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    class Join : Attribute
    {
        public Channel channel { get; set; }

        public Join(String channel)
        {
            this.channel = new Channel(channel);
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    class Leave : Attribute
    {
        public Channel channel { get; set; }

        public Leave(String channel)
        {
            this.channel = new Channel(channel);
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    class AutoResponse : Attribute
    {
    }

	public partial class Packet
    {
        [AttributeUsage(AttributeTargets.Field)]
        protected class MemberOf : Attribute
        {
            public Type type { get; set; }

            public MemberOf(Type type)
            {
                this.type = type;

                if (!type.IsSubclassOf(typeof(Model)))
                    throw new ArgumentException("type is not subclass of Model");
            }
        }
        [AttributeUsage(AttributeTargets.Field)]
        protected class KeyOf : Attribute
        {
            public Type type { get; set; }

            public KeyOf(Type type)
            {
                this.type = type;

                if (!type.IsSubclassOf(typeof(Model)))
                    throw new ArgumentException("type is not subclass of Model");
            }
        }
        [AttributeUsage(AttributeTargets.Field)]
        protected class Bind : Attribute
        {
            public String format { get; set; }

            public Bind(String format)
            {
                this.format = format;
            }
        }

        /// <summary>
        /// 패킷의 필드가 서버->클라이언트에 사용됨을 나타내는 속성
        /// </summary>
        [AttributeUsage(AttributeTargets.Field)]
		protected class S2C : Attribute
        {
        }
        /// <summary>
        /// 패킷의 필드가 클라이언트->서버에 사용됨을 나타내는 속성
        /// </summary>
        [AttributeUsage(AttributeTargets.Field)]
        protected class C2S : Attribute
        {
        }
    }
}
