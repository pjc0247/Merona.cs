using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class ServiceRouteTest
    {
        class TestPacket : Packet
        {
        }
        class TestService : Service
        {
            [Handler("a.b.c.d")]
            public void OnABCD(Session session, Packet packet)
            {
            }
            [Handler("a.b.c.*")]
            public void OnABC(Session session, Packet packet)
            {
            }

            [Handler(typeof(TestPacket))]
            public void OnTestPacket(Session session, TestPacket packet)
            {
            }
        }

        [TestMethod]
        public void RoutingByChannel()
        {
            var service = new TestService();

            var routed = service.Route(
                new Packet()
                {
                    channel = "a.b.c.d"
                });

            Assert.IsTrue(routed);
        }
        [TestMethod]
        public void RoutingByChannelWithWildcard()
        {
            var service = new TestService();

            var routed = service.Route(
                new Packet()
                {
                    channel = "a.b.c.xxx"
                });
            Assert.IsTrue(routed);

            routed = service.Route(
                new Packet()
                {
                    channel = "a.b.c.zzz"
                });
            Assert.IsTrue(routed);
        }

        [TestMethod]
        public void RoutingByPacketType()
        {
            var service = new TestService();

            var routed = service.Route(
                new TestPacket());

            Assert.IsTrue(routed);
        }

        [TestMethod]
        public void NotRouted()
        {
            var service = new TestService();

            var routed = service.Route(
                new Packet()
                {
                    channel = "never.exist.channel"
                });

            Assert.IsFalse(routed);
        }
    }
}
