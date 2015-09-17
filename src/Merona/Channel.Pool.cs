using System;
using System.Collections.Generic;

namespace Merona
{
    public sealed partial class Channel
    {
        public class Pool
        {
            [ThreadStatic]
            public static Pool current;

            private static TreeDictionary pool;

            static Pool()
            {
                pool = new TreeDictionary();
            }

            public void Join(Channel.Path path, Session session)
            {
                if (!path.isFixed)
                    throw new ArgumentException("path isFixed(false)");

                var targets = pool.Query(path);
                if(targets.Count > 0)
                {
                    foreach (var target in targets)
                        target.Join(session);
                }
                else
                {
                    pool.Add(path).Join(session);
                }
            }
            public void Leave(Channel.Path path, Session session)
            {
                if (!path.isFixed)
                    throw new ArgumentException("path isFixed(false)");

                var targets = pool.Query(path);
                if (targets.Count > 0)
                {
                    foreach (var target in targets)
                        target.Leave(session);
                }
            }
            
            /// <summary>
            /// 지정된 채널 경로에 포함되는 모든 세션을 얻어온다.
            /// [Non-Thread-Safe]
            /// </summary>
            /// <param name="path">채널 경로</param>
            /// <returns>포함된 모든 세션</returns>
            public HashSet<Session> Query(Channel.Path path)
            {
                HashSet<Session> results = new HashSet<Session>();

                foreach (var channel in pool.Query(path))
                {
                    foreach (var session in channel.Query())
                        results.Add(session);
                }

                return results;
            }

            /// <summary>
            /// 지정된 채널 경로에 패킷을 브로드캐스팅한다.
            /// [Non-Thread-Safe]
            /// </summary>
            /// <param name="path">채널 경로</param>
            /// <param name="packet">패킷</param>
            public void Broadcast(String path, Packet packet)
            {
                Broadcast(new Channel.Path(path), packet);
            }
            /// <summary>
            /// 지정된 채널 경로에 패킷을 브로드캐스팅한다.
            /// [Non-Thread-Safe]
            /// </summary>
            /// <param name="path">채널 경로</param>
            /// <param name="packet">패킷</param>
            public void Broadcast(Channel.Path path, Packet packet)
            {
                packet.channel = path.path;

                foreach(var session in Query(path))
                    session.Send(packet);
            }
        }

    }
}
