﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Session
    {
		/// <summary>
        /// 클라이언트로부터 요청 패킷을 수신하고,
        /// 처리해서 전송하기까지의 데이터를 이곳에 저장합니다.
        /// </summary>
		public class PipelineContext
        {
			public Packet request { get; internal set; }

        }

		public PipelineContext pipelineContext { get; private set; }
    }
}
