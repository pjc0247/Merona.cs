using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.EnvBased
{
    [TestClass]
    public class SchedulerTest : ServerBasedTest
    {
        [TestMethod]
        public void IsSafeThread()
        {
            object obj = new object();
            bool isSafeThread = false;

            server.scheduler.Defer(() =>
            {
                isSafeThread = Server.isSafeThread;
                lock (obj)
                {
                    Monitor.Pulse(obj);
                }
            });

            lock (obj)
            {
                Monitor.Wait(obj);
            }

            Assert.IsTrue(
                isSafeThread);
        }
    }
}
