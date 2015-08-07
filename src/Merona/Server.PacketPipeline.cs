using System;
using System.Collections.Generic;
using System.Text;

namespace Merona
{
	public sealed partial class Server
	{
		public delegate void PacketProcessor(Session session, Packet packet);

        internal SortedSet<Tuple<int, PacketProcessor>> preProcessors { get; set; }
        internal SortedSet<Tuple<int, PacketProcessor>> postProcessors { get; set; }

        private void InitializePipeline()
        {
            preProcessors = new SortedSet<Tuple<int, PacketProcessor>>();
            postProcessors = new SortedSet<Tuple<int, PacketProcessor>>();
        }
        
        public void AddPreProcessor(PacketProcessor processor, int priority)
        {
            preProcessors.Add(new Tuple<int, PacketProcessor>(priority, processor));
        }
        public void AddPostProcessor(PacketProcessor processor, int priority)
        {
            postProcessors.Add(new Tuple<int, PacketProcessor>(priority, processor));
        }
	}
}
