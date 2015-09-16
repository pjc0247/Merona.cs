using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace Merona
{
    /// <summary>
    /// 패킷의 아이디를 지정한다.
    /// 일반적으로 이 속성은 Merona.Pgen.cs에 의해
    /// 자동으로 추가된다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketId : Attribute
    {
        public int id { get; set; }
        public PacketId(int id)
        {
            this.id = id;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class Join : Attribute
    {
        public Channel.Path path { get; set; }

        public Join(String path)
        {
            this.path = new Channel.Path(path);
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class Leave : Attribute
    {
        public Channel.Path path { get; set; }

        public Leave(String path)
        {
            this.path = new Channel.Path(path);
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AutoResponse : Attribute
    {
        public Type type { get; set; }
        public Channel.Path path { get; set; }

        public AutoResponse(Type type, String path)
        {
            this.type = type;
            this.path = new Channel.Path(path);
        }
        public AutoResponse(Type type)
        {
            this.type = type;
        }
    }

	public partial class Packet
    {
        [AttributeUsage(AttributeTargets.Field)]
        protected class Forward : Attribute
        {
        }

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

        /// <summary>
        /// 패킷의 필드가 전송되기 이전에 SHA256으로 암호화되어야 함을 나타내는 속성.
        /// </summary>
        protected class Sha256 : CustomDescriptor
        {
            internal protected override void OnPostProcess(ref object target)
            {
                var msg = (String)target;

                var crypt = SHA256Managed.Create();
                var hash = new StringBuilder();

                var crypto = crypt.ComputeHash(
                    Encoding.UTF8.GetBytes(msg), 0, Encoding.UTF8.GetByteCount(msg));

                foreach (byte _byte in crypto)
                    hash.Append(_byte.ToString("x2"));

                target = hash.ToString();
            }
        }
    }
}
