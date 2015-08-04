using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingPackets
{
    [PgenTarget]
    public class Packets
    {
        public class Join
        {
            [C2S]
            public String nickname;
        }

        public class Leave
        {
        }

        public class ChatMessage
        {
            public String message;

            [S2C]
            public String nickname;
        }
    }
}
