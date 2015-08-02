using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class CircularBufferTest
    {
        [TestMethod]
        public void TestSimple()
        {
            var buffer = new CircularBuffer<int>(16);
            var data = new int[8];

            buffer.Put(data, 0, 8);

            Assert.AreEqual(
                8,
                buffer.Size);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalBufferOverflowException))]
        public void OverflowException()
        {
            var buffer = new CircularBuffer<int>(4);

            for (int i = 0; i < 32; i++)
                buffer.Put(i);
        }

        [TestMethod]
        public void Peek()
        {
            var buffer = new CircularBuffer<int>(16);

            for(var i=0;i<8;i++)
                buffer.Put(i);

            var peek1 = new int[8];
            var peek2 = new int[8];

            buffer.Peek(peek1, 0, 8);
            buffer.Peek(peek2, 0, 8);

            for(var i = 0; i < 8; i++)
            {
                Assert.AreEqual(
                    peek1[i],
                    peek2[i]);
            }
        }
    }
}
