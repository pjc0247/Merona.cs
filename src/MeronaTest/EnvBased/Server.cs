using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.EnvBased
{
    [TestClass]
    public class ServerTest : ServerBasedTest
    {
        
        [TestMethod]
        public void IsRunning()
        {
            Assert.IsTrue(
                server.isRunning);
        }

        [TestMethod]
        public void ThreadStatics()
        {
            Assert.IsNotNull(
                Server.current);

            Assert.IsNotNull(
                Scheduler.current);
            Assert.IsNotNull(
                Channel.Pool.current);
        }
    }
}
