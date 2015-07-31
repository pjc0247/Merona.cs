using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class ChannelTest
    {
        [TestMethod]
        public void IsMatch()
        {
            var ch1 = new Channel("some.path.*.hello");
            var ch2 = new Channel("some.path.aa.hello");

            Assert.IsTrue(
                ch1.IsMatch(ch2));
        }

        [TestMethod]
        public void DynamicPath()
        {
            var ch1 = Channel.makeDynamicPath.some.path.all.hello.fin;
            var ch2 = Channel.makeDynamicPath.some.path.aa.hello.fin;

            Assert.IsTrue(
                ch1.IsMatch(ch2));
        }
    }
}
