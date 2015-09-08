using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]    
    public class SafeCollection
    {
        [TestMethod]
        public void AutoRemove()
        {
            var sc = new SafeCollection<Session>();

            var session = new Session();

            sc.Add(session);

            session.Disconnect();

            Assert.AreEqual(
                sc.Count,
                0);
        }

    }
}
