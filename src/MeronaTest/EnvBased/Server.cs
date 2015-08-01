using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
