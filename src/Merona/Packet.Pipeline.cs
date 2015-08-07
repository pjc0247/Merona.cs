﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Merona
{
	public partial class Packet
	{
        /// <summary>
        /// 현재 패킷과, 지정된 세션에 대해서 인바인딩을 수행한다.
        /// (패킷 필드의 값을 세션의 프로퍼티에 바인딩)
        /// </summary>
        /// <param name="session">세션</param>
        private void InBind(Session session)
        {
            var bindFields = GetBindFields(GetType());

            foreach (var field in bindFields)
            {
                var value = field.Item2.GetValue(this);
                DataBinder.InBind(field.Item1, value, session);
            }
        }
        /// <summary>
        /// 현재 패킷과, 지정된 세션에 대해서 아웃바인딩을 수행한다.
        /// (세션 프로퍼티의 값을 패킷의 필드에 바인딩한다)
        /// </summary>
        /// <param name="session">세션</param>
        private void OutBind(Session session)
        {
            var bindFields = GetBindFields(GetType());

            foreach (var field in bindFields)
            {
                DataBinder.OutBind(field.Item1, session, field.Item2, this);
            }
        }
        /// <summary>
        /// 패킷이 각 서비스들에게 라우팅 되기 전 처리해야 할 작업들을 수행한다.
        /// 이 작업은 C2S 패킷에만 수행되어야 한다.
        /// </summary>
        /// <param name="session">세션</param>
        internal void PreProcess(Session session)
        {
            var join = Packet.GetJoinPath(GetType());
            if (join != null)
                Server.current.channelPool.Join(join, session);

            var leave = Packet.GetJoinPath(GetType());
            if (leave != null)
                Server.current.channelPool.Leave(leave, session);

            InBind(session);

            foreach (var pp in Server.current.preProcessors)
                pp.Item2.Invoke(session, this);
        }
        /// <summary>
        /// 패킷이 클라이언트에게 Send 되기 전 처리해야 할 작업들을 수행한다.
        /// 이 작업은 S2C 패킷에만 수행되어야 한다.
        /// </summary>
        /// <param name="session">세션</param>
        internal void PostProcess(Session session)
        {
            OutBind(session);

            foreach (var pp in Server.current.preProcessors)
                pp.Item2.Invoke(session, this);
        }
	}
}
