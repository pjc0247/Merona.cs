using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Merona
{
    public partial class Channel
    {
        public partial class Path
        {
            /// <summary>
            /// DynamicObject 기능을 제공해서 오브젝트 체인으로
            /// 채널 경로를 만들 수 있는 기능을 제공한다.
            /// </summary>
            /// <example>
            /// var path = Channel.Path.makeDynamicPath.chat.all.fin;
            /// </example>
            public class DynamicPathMaker : DynamicObject
            {
                private List<string> tokens { get; set; }

                public DynamicPathMaker()
                {
                    tokens = new List<string>();
                }

                public Path fin
                {
                    get
                    {
                        var path = String.Join(".", tokens);
                        return new Path(path);
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
        }
    }
}
