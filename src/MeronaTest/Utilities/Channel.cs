using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    class ChannelTest
    {
        [TestMethod]
        public void Query()
        {
            var ch = new Channel("world");

            ch.Join(new Session());
            ch.Join(new Session());

            Assert.AreEqual(
                2,
                ch.Query().Count);
        }
    }
}
