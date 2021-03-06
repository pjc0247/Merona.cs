﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Merona
{
    public class Service
    {
        /// <summary>
        /// 메소드가 특정한 패킷 또는 채널 경로에 대한 핸들러임을 나타내는 속성
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class Handler : Attribute
        {
            public Type type { get; set; }
            public Channel channel { get; set; }

            public Handler(Type type)
            {
                this.type = type;
            }
            public Handler(String channelPath)
            {
                this.channel = new Channel(channelPath);
            }
        }

        private Dictionary<Type, MethodBase> packetTypeRoutingTable { get;set; }
        private Dictionary<Channel, MethodBase> channelRoutingTable { get; set; }
        public Server server { get; internal set; }

        protected Service()
        {
            this.packetTypeRoutingTable = new Dictionary<Type, MethodBase>();
            this.channelRoutingTable = new Dictionary<Channel, MethodBase>();

            foreach (var method in GetType().GetMethods())
            {
                foreach (var _attr in method.GetCustomAttributes(typeof(Handler), false))
                {
                    var attr = (Handler)_attr;

                    if (attr.type != null)
                        packetTypeRoutingTable[attr.type] = method;
                    else if (attr.channel != null)
                        channelRoutingTable[attr.channel] = method;
                    else
                        throw new InvalidOperationException();
                }
            }
        }

        private object InvokeRouter(MethodBase router, object[] args)
        {
            Packet resp = null;

            try
            {
                resp = (Packet)router.Invoke(this, args);
            }
            catch(Exception e)
            {
                server.watcher.OnUserException(e);
            }
            
            if (resp != null)
            {
                /* send-back */
                var session = (Session)args[0];

                session.Send(resp);
            }

            return resp;
        }

        /// <summary>
        /// 주어진 패킷을 현재 서비스 내에서 라우팅한다.
        /// 만약 등록된 핸들러가 없을 경우 false를 반환한다.
        /// </summary>
        /// <param name="packet">패킷</param>
        /// <returns>현재 서비스에서 라우팅 되었는지 여부</returns>
        internal bool Route(Packet packet)
        {
            var invokeArg = new object[] { Session.current, packet };
            var routed = false;

            /* 채널 라우팅 */
            if (packet.channel != null)
            {
                foreach (var rule in channelRoutingTable)
                {
                    /* TODO : fixme */
                    if (rule.Key.IsMatch(packet.channel))
                    {
                        InvokeRouter(rule.Value, invokeArg);
                        routed = true;
                    }
                }
            }
            /* 패킷 타입 라우팅 */
            if (packetTypeRoutingTable.ContainsKey(packet.GetType()))
            {
                InvokeRouter(packetTypeRoutingTable[packet.GetType()], invokeArg);
                routed = true;
            }

            return routed;
        }

        /// <summary>
        /// 상속하여 서비스의 초기화 루틴을 구현한다.
        /// 이 메소드는 안전한 스레드에서 실행된다.
        /// </summary>
        internal protected virtual void Setup()
        {
        }
        /// <summary>
        /// 상속하여 서비스의 종료 루틴을 구현한다.
        /// 이 메소드는 안전한 스레드에서 실행된다.
        /// </summary>
        internal protected virtual void Cleanup()
        {
        }
    }
}
