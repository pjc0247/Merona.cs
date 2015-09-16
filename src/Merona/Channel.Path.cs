using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Merona
{
    public sealed partial class Channel
    {
        public partial class Path
        {
            internal static readonly int asterisk = "*".GetHashCode();
            internal static readonly int doubleAsterisk = "**".GetHashCode();

            public string path { get; private set; }
            public bool isFixed { get; private set; }
            internal int[] raw { get; private set; }

            public static Path makePath(string path)
            {
                return new Path(path);
            }
            public static dynamic makeDynamicPath
            {
                get
                {
                    return new DynamicPathMaker();
                }
            }
            private static int[] ToRawPath(string path, ref bool isFixed)
            {
                var tokens = path.Split('.');
                var raw = new int[tokens.Length];

                for (var i = 0; i < tokens.Length; i++)
                {
                    raw[i] = tokens[i].GetHashCode();

                    if (raw[i] == asterisk ||
                       raw[i] == doubleAsterisk)
                        isFixed = false;
                }

                return raw;
            }

            public Path(string path)
            {
                bool isFixed = true;

                this.path = path;
                this.raw = ToRawPath(path, ref isFixed);
                this.isFixed = isFixed;
            }

            public bool IsMatch(String inPath)
            {
                return IsMatch(Path.makePath(inPath));
            }
            public bool IsMatch(Path other)
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
}
