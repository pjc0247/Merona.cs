using System;

using Merona;

namespace ChattingServer
{
    class ChatService : Service
    {
        protected override void Setup()
        {
        }
        protected override void Cleanup()
        {
        }

        [Handler(typeof(Packets.Join.C2S))]
        public void JoinHandler(MySession session, Packets.Join.C2S packet)
        {
            session.nickname = packet.nickname;

            Console.WriteLine("join - {0}", packet.nickname);
        }

        [Handler(typeof(Packets.Leave.C2S))]
        public void LeaveHandler(MySession session, Packets.Leave.C2S packet)
        {
            Console.WriteLine("leave - {0}", session.nickname);
        }
    }
}