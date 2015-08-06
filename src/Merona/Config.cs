using System;

namespace Merona
{
    public class Config
    {
        public String name { get; set; }

        public String host { get; set; }
        public int port { get; set; }

        public int schedulerMaxInterval { get; set; }

        public Type sessionType { get; set; }
        public int sessionRingBufferSize { get; set; }
        public int sessionRecvBufferSize { get; set; }
        public int sessionPoolSize { get; set; }

        public String dbHostName { get; set; }
        public String dbDatabaseName { get; set; }

        public String endian { get; set; }

        public static Config defaults
        {
            get
            {
                return new Config();
            }
        }

        /// <summary>
        /// 기본 설정
        /// </summary>
        private Config()
        {
            this.name = "ServerName";
            this.host = "0.0.0.0";
            this.port = 9916;
            this.schedulerMaxInterval = 30;
            this.sessionRecvBufferSize = 128;
            this.sessionRingBufferSize = 1024;
            this.sessionPoolSize = 1024;
            this.dbHostName = "localhost";
            this.dbDatabaseName = "test";
            this.endian = "little"; // 기본값 big?
            this.sessionType = typeof(Session);
        }
        /// <summary>
        /// 파일로부터 설정값 생성
        /// </summary>
        /// <param name="path">설정 파일 경로</param>
        public Config(String path)
        {
            throw new NotImplementedException();
        }
    }
}
