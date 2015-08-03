using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Linq;

namespace Merona
{
    public sealed partial class Channel
    {
        public Path path { get; private set; }
        private List<WeakReference<Session>> sessions { get; set; }

        public Channel(Path path)
        {
            this.path = path;
            this.sessions = new List<WeakReference<Session>>();
        }
        public Channel(String path)
        {
            this.path = new Path(path);
            this.sessions = new List<WeakReference<Session>>();
        }

        public bool IsMatch(String inPath)
        {
            return path.IsMatch(inPath);
        }
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
            sessions.Add(new WeakReference<Session>(session));
        }
        /// <summary>
        /// 세션을 현재 채널에서 탈퇴시칸다.
        /// [Non-Thread-Safe]
        /// </summary>
        /// <param name="session">세션</param>
        public void Leave(Session session)
        {
            for(var i = 0; i < sessions.Count; i++)
            {
                Session dst;

                if(sessions[i].TryGetTarget(out dst))
                {
                    if(dst == session)
                    {
                        sessions.RemoveAt(i);
                        return;
                    }
                }
            }

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

            foreach(var weakSession in sessions)
            {
                Session dst;

                if(weakSession.TryGetTarget(out dst))
                    results.Add(dst);
            }

            return results;
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
