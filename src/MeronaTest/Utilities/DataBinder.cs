using System;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class DataBinderTest
    {
        class HelloWorldWithProperty
        {
            public String hello { get; set; }
            public String world { get; set; }
        }
        class HelloWorldWithField
        {
            public String hello;
            public String world;
        }

        [TestMethod]
        public void BindSimpleWithProperty()
        {
            var source = new HelloWorldWithProperty();

            source.hello = "hello";
            source.world = "world";

            Assert.AreEqual(
                "hello world",
                DataBinder.Bind(
                    "#{hello} #{world}", source,
                    DataBinder.SourceAttributeType.Property));
        }

        [TestMethod]
        public void BindSimpleWithField()
        {
            var source = new HelloWorldWithField();

            source.hello = "hello";
            source.world = "world";

            Assert.AreEqual(
                "hello world",
                DataBinder.Bind(
                    "#{hello} #{world}", source,
                    DataBinder.SourceAttributeType.Field));
        }
        
        [TestMethod]
        public void BindFailure()
        {
            /* 실패한 매칭은 치환되지 않음 */
            Assert.AreEqual(
                "#{not} #{exists}",
                DataBinder.Bind(
                    "#{not} #{exists}", new object(),
                    DataBinder.SourceAttributeType.Property));
        }
    }
}
