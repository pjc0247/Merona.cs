using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace Merona
{
    public class Config
    {
        public int schedulerMaxInterval { get; set; }
        public int sessionRingBufferSize { get; set; }
        public int sessionRecvBufferSize { get; set; }

        public String dbHostName { get; set; }
        public String dbDatabaseName { get; set; }

        public String endian { get; set; }

        /// <summary>
        /// 기본 설정
        /// </summary>
        public Config()
        {
            this.schedulerMaxInterval = 30;
            this.sessionRecvBufferSize = 128;
            this.sessionRingBufferSize = 1024;
            this.dbHostName = "localhost";
            this.dbDatabaseName = "test";
            this.endian = "little"; // 기본값 big?
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
