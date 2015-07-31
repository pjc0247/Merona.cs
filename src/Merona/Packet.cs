using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Merona
{
    public partial class Packet
    {
        public String channel = null;

        private static Dictionary<Type, List<Tuple<String, FieldInfo>>> keys;
        private static Dictionary<Type, List<Tuple<String, FieldInfo>>> binds;
        private static Dictionary<Type, List<FieldInfo>> c2s;
        private static Dictionary<Type, List<FieldInfo>> s2c;

        static Packet()
        {
            keys = new Dictionary<Type, List<Tuple<String, FieldInfo>>>();
            binds = new Dictionary<Type, List<Tuple<String, FieldInfo>>>();
            c2s = new Dictionary<Type, List<FieldInfo>>();
            s2c = new Dictionary<Type, List<FieldInfo>>();

            var packets = Assembly.GetEntryAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Packet)));

            foreach (var packet in packets)
            {
                foreach (var field in packet.GetFields())
                {
                    /* TODO : 정리 */

                    var memberOf = (Packet.MemberOf)field.GetCustomAttribute(typeof(Packet.MemberOf));
                    if (memberOf != null)
                    {
                        //continue;
                    }

                    var keyOf = (Packet.KeyOf)field.GetCustomAttribute(typeof(Packet.KeyOf));
                    if(keyOf != null)
                    {
                        if (!keys.ContainsKey(packet))
                            keys[packet] = new List<Tuple<String, FieldInfo>>();

                        keys[packet].Add(new Tuple<String, FieldInfo>(keyOf.type.Name, field));
                        //continue;
                    }

                    var bind = (Packet.Bind)field.GetCustomAttribute(typeof(Packet.Bind));
                    if(bind != null)
                    {
                        if (!binds.ContainsKey(packet))
                            binds[packet] = new List<Tuple<String, FieldInfo>>();

                        binds[packet].Add(new Tuple<String, FieldInfo>(bind.format, field));
                    }

                    var cts = (Packet.C2S)field.GetCustomAttribute(typeof(Packet.C2S));
                    if (cts != null)
                    {
                        if (!c2s.ContainsKey(packet))
                            c2s[packet] = new List<FieldInfo>();

                        c2s[packet].Add(field);
                    }
                    var stc = (Packet.S2C)field.GetCustomAttribute(typeof(Packet.S2C));
                    if (stc != null)
                    {
                        if (!s2c.ContainsKey(packet))
                            s2c[packet] = new List<FieldInfo>();

                        s2c[packet].Add(field);
                    }
                }
            }
        }

        public static List<Tuple<String, FieldInfo>> GetKeyFields<T>() where T : Packet
        {
            if (!keys.ContainsKey(typeof(T)))
                return null;
            return keys[typeof(T)];
        }
        public static List<Tuple<String, FieldInfo>> GetKeyFields(Type type)
        {
            if (!keys.ContainsKey(type))
                return null;
            return keys[type];
        }

        public static List<Tuple<String, FieldInfo>> GetBindFields<T>() where T : Packet
        {
            if (!binds.ContainsKey(typeof(T)))
                return null;
            return binds[typeof(T)];
        }
        public static List<Tuple<String, FieldInfo>> GetBindFields(Type type)
        {
            if (!binds.ContainsKey(type))
                return null;
            return binds[type];
        }

        public static List<FieldInfo> GetC2SFields<T>() where T : Packet
        {
            if (!c2s.ContainsKey(typeof(T)))
                return null;
            return c2s[typeof(T)];
        }
        public static List<FieldInfo> GetC2SFields(Type type)
        {
            if (!c2s.ContainsKey(type))
                return null;
            return c2s[type];
        }
        public static List<FieldInfo> GetS2CFields<T>() where T : Packet
        {
            if (!c2s.ContainsKey(typeof(T)))
                return null;
            return s2c[typeof(T)];
        }
        public static List<FieldInfo> GetS2CFields(Type type)
        {
            if (!s2c.ContainsKey(type))
                return null;
            return s2c[type];
        }

        internal void PostProcess()
        {
            var bindFields = GetBindFields(GetType());

            if(bindFields == null)
                return;

            foreach (var field in bindFields)
            {
                var bound = DataBinder.Bind(field.Item1, Session.current);

                field.Item2.SetValue(this, bound);
            }
        }
    }
}
