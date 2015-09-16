using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Merona
{
    public partial class Channel
    {
        public partial class Path
        {
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
