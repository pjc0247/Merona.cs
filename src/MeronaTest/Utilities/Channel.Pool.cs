using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class ChannelPoolTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonFixedPathJoin()
        {
            var pool = new Channel.Pool();

            pool.Join(new Channel.Path("world.*"), new Session());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonFixedPathLeave()
        {
            var pool = new Channel.Pool();
            var session = new Session();

            pool.Join(new Channel.Path("world.map"), session);
            pool.Leave(new Channel.Path("world.*"), session);
        }
    }
}
