using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class ChannelPathTest
    {
        [TestMethod]
        public void IsMatch()
        {
            var ch1 = new Channel.Path("some.path.*.hello");
            var ch2 = new Channel.Path("some.path.aa.hello");

            Assert.IsTrue(
                ch1.IsMatch(ch2));
        }

        [TestMethod]
        public void DynamicPath()
        {
            var ch1 = Channel.Path.makeDynamicPath.some.path.all.hello.fin;
            var ch2 = Channel.Path.makeDynamicPath.some.path.aa.hello.fin;

            Assert.IsTrue(
                ch1.IsMatch(ch2));
        }
    }
}
