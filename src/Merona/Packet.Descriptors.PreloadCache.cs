using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using NLog;

namespace Merona
{
    public partial class Packet
    {
        private static Dictionary<Type, List<Tuple<String, FieldInfo>>> keys;
        private static Dictionary<Type, List<Tuple<String, FieldInfo>>> binds;
        private static Dictionary<Type, List<Tuple<Packet.CustomDescriptor, FieldInfo>>> customs;
        private static Dictionary<Type, List<FieldInfo>> c2s;
        private static Dictionary<Type, List<FieldInfo>> s2c;
        private static Dictionary<Type, List<FieldInfo>> forwards;
        private static Dictionary<Type, AutoResponse> autoResponses;
        private static Dictionary<Type, Channel.Path> joins;
        private static Dictionary<Type, Channel.Path> leaves;

        private static Dictionary<int, Type> types;

        /// <summary>
        /// 모든 패킷들에 대한 속성을 읽어서 캐시를 만든다.
        /// </summary>
		private static void InitializePreloadCaches()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            logger.Info("Packet::InitializePreloadCaches");

            keys = new Dictionary<Type, List<Tuple<String, FieldInfo>>>();
            binds = new Dictionary<Type, List<Tuple<String, FieldInfo>>>();
            customs = new Dictionary<Type, List<Tuple<CustomDescriptor, FieldInfo>>>();
            c2s = new Dictionary<Type, List<FieldInfo>>();
            s2c = new Dictionary<Type, List<FieldInfo>>();
            forwards = new Dictionary<Type, List<FieldInfo>>();

            joins = new Dictionary<Type, Channel.Path>();
            leaves = new Dictionary<Type, Channel.Path>();

            types = new Dictionary<int, Type>();
            autoResponses = new Dictionary<Type, AutoResponse>();

            var packets = Assembly.GetEntryAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Packet)));

            foreach (var packet in packets)
            {
                var id = (PacketId)packet.GetCustomAttribute(typeof(PacketId));
                if (id != null)
                    types[id.id] = packet;

                var autoResponse = (AutoResponse)packet.GetCustomAttribute<AutoResponse>();
                if (autoResponse != null)
                    autoResponses[packet] = autoResponse;

                var join = (Join)packet.GetCustomAttribute<Join>();
                if (join != null)
                    joins[packet] = join.path;

                var leave = (Leave)packet.GetCustomAttribute<Leave>();
                if (leave != null)
                    leaves[packet] = leave.path;

                foreach (var field in packet.GetFields())
                {
                    /* TODO : 정리 */
                    var customAttrs = field.GetCustomAttributes(typeof(Packet.CustomDescriptor), true);
                    foreach (var custom in customAttrs)
                    {
                        if (!customs.ContainsKey(packet))
                            customs[packet] = new List<Tuple<Packet.CustomDescriptor, FieldInfo>>();

                        customs[packet].Add(new Tuple<CustomDescriptor, FieldInfo>((Packet.CustomDescriptor)custom, field));
                    }

                    var forward = (Packet.Forward)field.GetCustomAttribute(typeof(Packet.Forward));
                    if (forward != null)
                    {
                        if (!forwards.ContainsKey(packet))
                            forwards[packet] = new List<FieldInfo>();

                        forwards[packet].Add(field);
                    }

                    var memberOf = (Packet.MemberOf)field.GetCustomAttribute(typeof(Packet.MemberOf));
                    if (memberOf != null)
                    {
                        //continue;
                    }

                    var keyOf = (Packet.KeyOf)field.GetCustomAttribute(typeof(Packet.KeyOf));
                    if (keyOf != null)
                    {
                        if (!keys.ContainsKey(packet))
                            keys[packet] = new List<Tuple<String, FieldInfo>>();

                        keys[packet].Add(new Tuple<String, FieldInfo>(keyOf.type.Name, field));
                        //continue;
                    }

                    var bind = (Packet.Bind)field.GetCustomAttribute(typeof(Packet.Bind));
                    if (bind != null)
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

            logger.Info("{0} packets processed", packets.Count());
        }

        public static List<FieldInfo> GetForwardFields<T>() where T : Packet
        {
            if (!forwards.ContainsKey(typeof(T)))
                return new List<FieldInfo>();
            return forwards[typeof(T)];
        }
        public static List<FieldInfo> GetForwardFields(Type type)
        {
            if (!forwards.ContainsKey(type))
                return new List<FieldInfo>();
            return forwards[type];
        }

        public static List<Tuple<String, FieldInfo>> GetKeyFields<T>() where T : Packet
        {
            if (!keys.ContainsKey(typeof(T)))
                return new List<Tuple<String, FieldInfo>>();
            return keys[typeof(T)];
        }
        public static List<Tuple<String, FieldInfo>> GetKeyFields(Type type)
        {
            if (!keys.ContainsKey(type))
                return new List<Tuple<String, FieldInfo>>();
            return keys[type];
        }

        public static List<Tuple<String, FieldInfo>> GetBindFields<T>() where T : Packet
        {
            if (!binds.ContainsKey(typeof(T)))
                return new List<Tuple<String, FieldInfo>>();
            return binds[typeof(T)];
        }
        public static List<Tuple<String, FieldInfo>> GetBindFields(Type type)
        {
            if (!binds.ContainsKey(type))
                return new List<Tuple<String, FieldInfo>>();
            return binds[type];
        }

        public static List<Tuple<Packet.CustomDescriptor, FieldInfo>> GetCustomFields<T>() where T : Packet
        {
            if (!customs.ContainsKey(typeof(T)))
                return new List<Tuple<Packet.CustomDescriptor, FieldInfo>>();
            return customs[typeof(T)];
        }
        public static List<Tuple<Packet.CustomDescriptor, FieldInfo>> GetCustomFields(Type type)
        {
            if (!customs.ContainsKey(type))
                return new List<Tuple<Packet.CustomDescriptor, FieldInfo>>();
            return customs[type];
        }

        public static List<FieldInfo> GetC2SFields<T>() where T : Packet
        {
            if (!c2s.ContainsKey(typeof(T)))
                return new List<FieldInfo>();
            return c2s[typeof(T)];
        }
        public static List<FieldInfo> GetC2SFields(Type type)
        {
            if (!c2s.ContainsKey(type))
                return new List<FieldInfo>();
            return c2s[type];
        }
        public static List<FieldInfo> GetS2CFields<T>() where T : Packet
        {
            if (!c2s.ContainsKey(typeof(T)))
                return new List<FieldInfo>();
            return s2c[typeof(T)];
        }
        public static List<FieldInfo> GetS2CFields(Type type)
        {
            if (!s2c.ContainsKey(type))
                return new List<FieldInfo>();
            return s2c[type];
        }


        public static AutoResponse GetAutoResponse<T>()
        {
            if (!autoResponses.ContainsKey(typeof(T)))
                return null;
            return autoResponses[typeof(T)];
        }
        public static AutoResponse GetAutoResponse(Type type)
        {
            if (!autoResponses.ContainsKey(type))
                return null;
            return autoResponses[type];
        }

        public static Channel.Path GetJoinPath<T>()
        {
            if (!joins.ContainsKey(typeof(T)))
                return null;
            return joins[typeof(T)];
        }
        public static Channel.Path GetJoinPath(Type type)
        {
            if (!joins.ContainsKey(type))
                return null;
            return joins[type];
        }
        public static Channel.Path GetLeavePath<T>()
        {
            if (!leaves.ContainsKey(typeof(T)))
                return null;
            return leaves[typeof(T)];
        }
        public static Channel.Path GetLeavePath(Type type)
        {
            if (!leaves.ContainsKey(type))
                return null;
            return leaves[type];
        }

        /// <summary>
        /// 패킷 아이디로부터 패킷 타입을 얻어온다.
        /// [Thread-Safe]
        /// </summary>
        /// <param name="id">패킷 아이디</param>
        /// <returns>패킷 타입</returns>
        public static Type GetTypeById(int id)
        {
            return types[id];
        }
    }
}
