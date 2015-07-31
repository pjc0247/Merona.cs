﻿using System;
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

        static Packet()
        {
            keys = new Dictionary<Type, List<Tuple<String, FieldInfo>>>();
            binds = new Dictionary<Type, List<Tuple<String, FieldInfo>>>();

            var packets = Assembly.GetEntryAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Packet)));

            foreach (var packet in packets)
            {
                foreach (var field in packet.GetFields())
                {
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

        public static void PostProcess(ref Packet packet)
        {
            var bindFields = GetBindFields(packet.GetType());

            if(bindFields == null)
                return;

            foreach (var field in bindFields)
            {
                var bound = DataBinder.Bind(field.Item1, Session.current);

                field.Item2.SetValue(packet, bound);
            }
        }

        public void Dump()
        {
            Console.WriteLine(GetType().Name);
            foreach(var field in GetType().GetFields())
            {
                Console.WriteLine("  {0} : {1}", field.Name, field.GetValue(this));
            }
        }
    }
}
