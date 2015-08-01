using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class ChannelTreeDictionaryTest
    {
        [TestMethod]
        public void Query()
        {
            var td = new Channel.TreeDictionary();

            td.Add(new Channel.Path("world.map.my_home"));
            td.Add(new Channel.Path("world.map.dungeon"));
            td.Add(new Channel.Path("world.map.field"));

            Assert.AreEqual(
                1,
                td.Query(new Channel.Path("world.map.my_home")).Count);
            Assert.AreEqual(
                0,
                td.Query(new Channel.Path("world.map.heaven")).Count);
        }

        [TestMethod]
        public void QueryWithAsterisk()
        {
            var td = new Channel.TreeDictionary();

            td.Add(new Channel.Path("world.map.my_home"));
            td.Add(new Channel.Path("world.map.dungeon"));
            td.Add(new Channel.Path("world.map.field"));

            Assert.AreEqual(
                3,
                td.Query(new Channel.Path("world.map.*")).Count);
        }
    }
}
