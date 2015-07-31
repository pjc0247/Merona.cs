using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Merona
{
    /// <summary>
    /// 템플릿 문자열에 오브젝트를 바인딩할 수 있는 기능을 제공한다.
    /// </summary>
    /// <example>
    /// dynamic foo = new SomeDynamic();
    /// foo.world = "WoRold";
    /// DataBinder.Bind("hello #{world}", foo);
    /// </example>
    public sealed class DataBinder
    {
        private static String ResolvePropertyPath(String path, object source)
        {
            var tokens = path.Split('.');
            var current = source;

            foreach (var token in tokens)
            {
                var prop = current.GetType().GetProperty(token);

                if (prop == null)
                    return null;

                current = prop.GetValue(current);
            }

            return current.ToString();
        }
        public static String Bind(String format, object source)
        {
            var result = "";
            var sourceType = source.GetType();
            var innerBracket = -1;

            format += " "; //padding

            for (var i = 0; i < format.Length-1; i++)
            {
                if(innerBracket != -1){
                    if (format[i] == '}')
                    {
                        var key = format.Substring(innerBracket + 2, i - innerBracket - 2);
                        var value = ResolvePropertyPath(key, source);

                        if (value != null)
                        {
                            result += value;
                        }
                        else
                        {
                            result += "#{" + key + "}";
                        }

                        innerBracket = -1;
                    }
                }
                else{
                    if (format[i] == '#' && format[i + 1] == '{')
                        innerBracket = i;
                    else
                        result += format[i];
                }
            }

            return result;
        }

    }
}
