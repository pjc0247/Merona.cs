﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

using MongoDB.Bson;
using MongoDB.Driver;

using NLog;

namespace Merona
{
    public sealed partial class Server
    {
        [ThreadStatic]
        public static Server current = null;

        public IMongoDatabase database { get; private set; }
        public Socket socket
        {
            get { return listener.Server; }
        }
        public Logger logger { get; private set; }
        public Channel.Pool channelPool { get; private set; }
        public String name { get; private set; }

        /// <summary>
        /// 현재 서버가 실행중인지 조사한다.
        /// </summary>
        public bool isRunning { get; set; }
        /// <summary>
        /// 현재 스레드가 Server.current에 대해 안전한지 조사한다.
        /// </summary>
        public static bool isSafeThread
        {
            get
            {
                if (current == null)
                    return false;
                return current.worker.ThreadId == Thread.CurrentThread.ManagedThreadId;
            }
        }

        private Session.Pool sessionPool { get; set; }
        private List<Service> services { get; set; }
        private MongoClient mongoClient { get; set; }
        private TcpListener listener { get; set; }
        internal BlockingCollection<Tuple<Session,Packet>> pendingPackets { get; private set; }
        internal BlockingCollection<Session> pendingClients { get; private set; }

        private Worker worker { set; get; }

        public Server(String name = "")
        {
            this.name = name;
            this.logger = LogManager.GetLogger(name);
            this.worker = new Worker(this);
            this.services = new List<Service>();
            this.mongoClient = new MongoClient();
            this.database = mongoClient.GetDatabase("test"); /* TODO : config */
            this.listener = new TcpListener(9916); /* TODO : config */
            this.sessionPool = new Session.Pool(1000); /* TODO : config */
            this.channelPool = new Channel.Pool();

            this.pendingPackets = new BlockingCollection<Tuple<Session,Packet>>();
            this.pendingClients = new BlockingCollection<Session>();

            this.listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }

        /// <summary>
        /// 서버에 서비스를 추가한다.
        /// 이 동작은 서버가 실행중이 아닐 때만 수행할 수 있다.
        /// </summary>
        /// <param name="service">추가할 서비스</param>
        public void AttachService<T>(T service) where T : Service
        {
            /* 실행 중에는 서비스가 추가될 수 없음 */
            if (isRunning)
                throw new InvalidOperationException("");

            logger.Info("Attach {0}", typeof(T).Name);

            services.Add(service);
            service.server = this;
        }

        /* for test */
        /// <summary>
        /// 미처리 패킷 목록에 패킷을 담는다.
        /// 이 패킷들은 주기적으로 Worker에 의해서 Consume된다.
        /// </summary>
        /// <param name="session">패킷을 수신한 세션</param>
        /// <param name="packet">수신한 패킷</param>
        public void Enqueue(Session session, Packet packet)
        {
            pendingPackets.Add(new Tuple<Session,Packet>(session,packet));
        }

        /// <summary>
        /// 서버를 시작한다.
        /// </summary>
        public void Start()
        {
            if (isRunning)
                throw new InvalidOperationException("server already running");
            
            logger.Info("Start Server");

            listener.Start();
            worker.Start();

            logger.Info("Begin AcceptTcpClient");
            listener.BeginAcceptTcpClient(new AsyncCallback(Acceptor), null);

            isRunning = true;
        }

        public void Kill()
        {
            if (!isRunning)
                throw new InvalidOperationException();

            isRunning = false;

            worker.Kill();
            listener.Stop();
        }

        private void Acceptor(IAsyncResult result)
        {
            try
            {
                var client = listener.EndAcceptTcpClient(result);
                var session = sessionPool.Acquire();

                if (session != null)
                {
                    /* Worker가 두개 이상일 경우 OnAccept보다 일반 패킷이 먼저 처리될 가능성이 있음 */
                    session.Reset(client);
                    
                    pendingClients.Add(session);
                }
                else
                {
                    logger.Error("sessionPool underflow");
                }
            }
            catch (SocketException e)
            {
                logger.Error("Acceptor", e);
            }
            catch (ObjectDisposedException e)
            {
                logger.Error("Acceptor", e);
            }
            finally
            {
                if(isRunning)
                    listener.BeginAcceptSocket(new AsyncCallback(Acceptor), null);
            }
        }
    }
}
