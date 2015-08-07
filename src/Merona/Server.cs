using System;
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
        /// <summary>
        /// 현재 Worker를 가지고 있는 서버의 인스턴스
        /// 이 값은 Worekr 스레드에서만 유효하다
        /// </summary>
        [ThreadStatic]
        public static Server current = null;

        public IMongoDatabase database { get; private set; }
        public Socket socket
        {
            get { return listener.Server; }
        }
        public Logger logger { get; private set; }
        public Channel.Pool channelPool { get; private set; }
        public Scheduler scheduler { get; private set; }
        public Config config { get; private set; }
        
        private long _isRunning = 0;
        /// <summary>
        /// 현재 서버가 실행중인지 조사한다.
        /// </summary>
        public bool isRunning {
            get
            {
                if (Interlocked.Read(ref _isRunning) == 1)
                    return true;
                else
                    return false;
            }
            set
            {
                long _value = value ? 1 : 0;
                Interlocked.Exchange(ref _isRunning, _value);
            }
        }
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
        internal BlockingCollection<Event> pendingEvents { get; private set; }

        internal Worker worker { set; private get; }
        internal IoWorker ioWorker { get; private set; }

        public Server(Config config = null)
        {
            Server.current = this;

            if (config == null)
                config = Config.defaults;

            this.config = config;
            this.logger = LogManager.GetLogger(config.name);
            this.worker = new Worker(this);
            this.ioWorker = new IoWorker(this);
            this.scheduler = new Scheduler(this);
            this.services = new List<Service>();
            this.mongoClient = new MongoClient();
            this.database = mongoClient.GetDatabase(config.dbDatabaseName);
            this.listener = new TcpListener(config.port);
            this.sessionPool = new Session.Pool(config.sessionPoolSize);
            this.channelPool = new Channel.Pool();
            
            this.pendingEvents = new BlockingCollection<Event>();

            this.listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            InitializePipeline();
            InitializeMarshaler();
        }

        /// <summary>
        /// 서버에 서비스를 추가한다.
        /// 이 동작은 서버가 실행중이 아닐 때만 수행할 수 있다.
        /// [Non-Thread-Safe]
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
        
        /// <summary>
        /// 미처리 이벤트 목록에 이벤트를 담는다.
        /// 이 이벤트들은 주기적으로 Worker에 의해서 Consume된다.
        /// [Thread-Safe]
        /// </summary>
        /// <param name="ev">이벤트</param>
        internal void Enqueue(Event ev)
        {
            pendingEvents.Add(ev);
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
            scheduler.Start();
            ioWorker.Start();
            worker.Start();

            logger.Info("Begin AcceptTcpClient");
            listener.BeginAcceptTcpClient(new AsyncCallback(Acceptor), null);

            isRunning = true;
        }

        /// <summary>
        /// 서버를 종료한다.
        /// [Thread-Safe]
        /// </summary>
        public void Kill()
        {
            if (!isRunning)
                throw new InvalidOperationException();

            isRunning = false;

            ioWorker.Kill();
            worker.Kill();
            scheduler.Kill();
            listener.Stop();
        }

        private void Acceptor(IAsyncResult result)
        {
            try
            {
                var client = listener.EndAcceptTcpClient(result);
                var session = sessionPool.Acquire();

                pendingEvents.Add(new AcceptEvent(client));
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
