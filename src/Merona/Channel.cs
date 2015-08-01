using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace Merona
{
    public sealed partial class Channel
    {
        public class DynamicPathMaker : DynamicObject
        {
            private List<string> tokens { get; set; }

            public DynamicPathMaker()
            {
                tokens = new List<string>();
            }

            public Channel fin
            {
                get
                {
                    var path = String.Join(".", tokens);
                    return new Channel(path);
                }
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                string name = binder.Name;

                if (name == "fin")
                    result = fin;
                else if (name == "all")
                {
                    tokens.Add("*");
                    result = this;
                }
                else
                {
                    tokens.Add(binder.Name);
                    result = this;
                }

                return true;
            }
        }

        public static Channel makePath(string path)
        {
            return new Channel(path);
        }
        public static dynamic makeDynamicPath
        {
            get
            {
                return new DynamicPathMaker();
            }
        }
        private static int[] ToRawPath(string path)
        {
            var tokens = path.Split('.');
            var raw = new int[tokens.Length];

            for (var i = 0; i < tokens.Length; i++)
            {
                raw[i] = tokens[i].GetHashCode();
            }

            return raw;
        }
        internal static readonly int asterisk = "*".GetHashCode();
        internal static readonly int doubleAsterisk = "**".GetHashCode();

        public string path { get; private set; }
        internal int[] raw { get; private set; }

        public Channel(string path)
        {
            this.path = path;
            this.raw = ToRawPath(path);
        }

        public bool IsMatch(String inPath)
        {
            return IsMatch(Channel.makePath(inPath));
        }
        public bool IsMatch(Channel other)
        {
            /* TODO : doubleAsterick */
            if (raw.Length != other.raw.Length)
                return false;

            for (var i = 0; i < other.raw.Length; i++)
            {
                if (raw[i] == asterisk ||
                    other.raw[i] == asterisk)
                    continue;

                if (raw[i] != other.raw[i])
                    return false;
            }

            return true;
        }
    }
}
