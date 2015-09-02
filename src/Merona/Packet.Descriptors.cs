﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace Merona
{
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

        protected class Sha256 : CustomDescriptor
        {
            internal protected override void OnPostProcess(ref object target)
            {
                var msg = (String)target;

                Console.WriteLine(msg);
                var crypt = SHA256Managed.Create();
                var hash = new StringBuilder();

                var crypto = crypt.ComputeHash(
                    Encoding.UTF8.GetBytes(msg), 0, Encoding.UTF8.GetByteCount(msg));

                foreach (byte theByte in crypto)
                    hash.Append(theByte.ToString("x2"));

                target = hash.ToString();
            }
        }
    }
}
