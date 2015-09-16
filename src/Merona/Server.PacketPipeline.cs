using System;
using System.Collections.Generic;
using System.Text;

namespace Merona
{
	public sealed partial class Server
	{
		public delegate void PacketProcessor(Session session, Packet packet);

        private class Order : IComparer<Tuple<int, PacketProcessor>>
        {
            public int Compare(Tuple<int, PacketProcessor> x, Tuple<int, PacketProcessor> y)
            {
                if (x.Item1 > y.Item1)
                    return 1;
                else if (x.Item1 == y.Item1)
                    return 0;
                else
                    return -1;
            }
        }

        internal SortedSet<Tuple<int, PacketProcessor>> preProcessors { get; set; }
        internal SortedSet<Tuple<int, PacketProcessor>> postProcessors { get; set; }

        private void InitializePipeline()
        {
            preProcessors = new SortedSet<Tuple<int, PacketProcessor>>(new Order());
            postProcessors = new SortedSet<Tuple<int, PacketProcessor>>(new Order());
        }
        
        /// <summary>
        /// 패킷 전처리기를 등록한다.
        /// </summary>
        /// <param name="processor">등록할 전처리기</param>
        /// <param name="priority">우선 순위 (낮을 수록 먼저 실행됨)</param>
        public void AddPreProcessor(PacketProcessor processor, int priority)
        {
            preProcessors.Add(new Tuple<int, PacketProcessor>(priority, processor));
        }

        /// <summary>
        /// 패킷 후처리기를 등록한다.
        /// </summary>
        /// <param name="processor">등록할 전처리기</param>
        /// <param name="priority">우선 순위 (낮을 수록 먼저 실행됨)</param>
        public void AddPostProcessor(PacketProcessor processor, int priority)
        {
            postProcessors.Add(new Tuple<int, PacketProcessor>(priority, processor));
        }
	}
}
