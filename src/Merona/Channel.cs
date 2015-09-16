using System;
using System.Collections.Generic;

namespace Merona
{
    public sealed partial class Channel
    {
        public Path path { get; private set; }
        private HashSet<Session> sessions { get; set; }

        public Channel(Path path)
        {
            this.path = path;
            this.sessions = new HashSet<Session>();
        }
        public Channel(String path)
        {
            this.path = new Path(path);
            this.sessions = new HashSet<Session>();
        }

        /// <summary>
        /// 주어진 채널 경로와 현재 채널이 매치되는지 판별한다.
        /// [Non-Thread-Safe]
        /// </summary>
        /// <param name="inPath">판별할 채널 경로</param>
        /// <returns>매치되면 true, 아닐 경우 false</returns>
        public bool IsMatch(String inPath)
        {
            return path.IsMatch(inPath);
        }
        /// <summary>
        /// 주어진 채널과 현재 채널이 매치되는지 판별한다.
        /// [Non-Thread-Safe]
        /// </summary>
        /// <param name="other">판별할 채널</param>
        /// <returns>매치되면 true, 아닐 경우 false</returns>
        public bool IsMatch(Channel other)
        {
            return path.IsMatch(other.path);
        }

        /// <summary>
        /// 세션을 현재 채널에 가입시킨다.
        /// [Non-Thread-Safe]
        /// </summary>
        /// <param name="session">세션</param>
        public void Join(Session session)
        {
            session.channels.Add(this);
            sessions.Add(session);
        }
        /// <summary>
        /// 세션을 현재 채널에서 탈퇴시칸다.
        /// [Non-Thread-Safe]
        /// </summary>
        /// <param name="session">세션</param>
        public void Leave(Session session)
        {
            if (sessions.Remove(session))
                session.channels.Remove(this);
            else
                Server.current.logger.Warn("Channel::Leave - session not found");
        }

        /// <summary>
        /// 현재 채널에 가입된 세션의 목록을 가져온다.
        /// [Non-Thread-Safe]
        /// </summary>
        /// <returns>가입된 세션들의 목록</returns>
        public List<Session> Query()
        {
            List<Session> results = new List<Session>();

            foreach (var session in sessions)
                results.Add(session);

            return results;
        }
        public void Broadcast(Packet packet)
        {
            foreach (var session in Query())
                session.Send(packet);
        }

        
        /// <summary>
        /// 빈 세션을 수집하여 정리한다.
        /// </summary>
        internal void Collect()
        {
            // TODO 

            Server.current.logger.Debug("Channel::Collect");
        }
    }
}
