using System;
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
        public enum SourceAttributeType
        {
            Property,
            Field
        }

        private static MemberInfo ResolveObjectPath(String path, object source, SourceAttributeType type)
        {
            var tokens = path.Split('.');
            var current = source;
            dynamic attr = null;

            foreach (var token in tokens)
            {
                if (type == SourceAttributeType.Field)
                    attr = current.GetType().GetField(token);
                else
                    attr = current.GetType().GetProperty(token);

                if (attr == null)
                    return null;

                current = attr.GetValue(current);
            }

            return attr;
        }

        public static String Bind(String format, object source, SourceAttributeType type)
        {
            var result = "";
            var sourceType = source.GetType();
            var innerBracket = -1;

            format += " "; //padding

            for (var i = 0; i < format.Length - 1; i++)
            {
                if (innerBracket != -1)
                {
                    if (format[i] == '}')
                    {
                        var key = format.Substring(innerBracket + 2, i - innerBracket - 2);
                        dynamic valueSource = ResolveObjectPath(key, source, type);

                        if (valueSource != null)
                        {
                            var value = valueSource.GetValue(source);
                            result += value;
                        }
                        else
                        {
                            result += "#{" + key + "}";
                        }

                        innerBracket = -1;
                    }
                }
                else
                {
                    if (format[i] == '#' && format[i + 1] == '{')
                        innerBracket = i;
                    else
                        result += format[i];
                }
            }

            return result;
        }

        // source -> Session
        // dest -> Packet's field
        public static void OutBind(String format, object source, FieldInfo destField, object dest)
        {
            var result = Bind(format, source, SourceAttributeType.Property);

            destField.SetValue(dest, result);
        }

        // source -> Packet's field
        // dest -> Session
        public static void InBind(String path, object source, object dest)
        {
            var prop = (PropertyInfo)ResolveObjectPath(path, dest, SourceAttributeType.Property);

            prop.SetValue(dest, source);
        }
    }
}
