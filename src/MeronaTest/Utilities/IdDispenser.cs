using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class IdDispenserTest
    {
        [TestMethod]
        public void MultiThreadDispense()
        {
            IdDispenser id = new IdDispenser();
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < 5; i++)
            {
                var t = new Thread(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        var _id = id.next;
                    }
                });
                t.Start();
                threads.Add(t);
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.AreEqual(
                5001,
                id.next);
        }
    }
}
