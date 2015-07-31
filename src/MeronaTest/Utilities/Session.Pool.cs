using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    class SessionPoolTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReturnOverflowException()
        {
            var pool = new Session.Pool(4);
            var session = pool.Acquire();
            pool.Return(session);
            pool.Return(session);
        }
    }
}
