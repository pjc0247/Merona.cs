using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Session
    {
		public class PipelineContext
        {
			public Packet request { get; internal set; }

        }

		public PipelineContext pipelineContext { get; private set; }
    }
}
