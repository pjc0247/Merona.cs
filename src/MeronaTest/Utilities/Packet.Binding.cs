using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class PacketBindingTest
    {
        class ChangeNickname : Packet
        {
            [Bind("nickname")]
            public String nickname;
        }
        class QueryNickname : Packet
        {
            [Bind("#{nickname}")]
            public String nickname;
        }
        class MySession : Session
        {
            public String nickname { get; set; }
        }

        [TestMethod]
        public void InBind()
        {
            var session = new MySession();
            var packet = new ChangeNickname();

            packet.nickname = "hello";
            packet.PreProcess(session);

            Assert.AreEqual(
                packet.nickname,
                session.nickname);
        }

        [TestMethod]
        public void OutBind()
        {
            var session = new MySession();
            var packet = new QueryNickname();

            session.nickname = "hello";
            packet.PostProcess(session);

            Assert.AreEqual(
                packet.nickname,
                session.nickname);
        }
    }
}
