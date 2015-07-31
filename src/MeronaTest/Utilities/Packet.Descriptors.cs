using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;
using System.Linq;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class PacketDescriptorsTest
    {
        class Player : Model
        {
            string name;

            int level;
        }

        public class QueryPlayer : Packet
        {
            [KeyOf(typeof(Player))]
            public string player_id;

            [MemberOf(typeof(Player))]
            public string name;
        }

        [TestMethod]
        public void KeyOf()
        {
            Assert.IsTrue(
                Packet.GetKeyFields<QueryPlayer>()
                /* Tuple<String, FieldInfo>
                 *    item1 : 타겟 Model의 클래스 이름
                 *    item2 : Packet의 FieldInfo
                 */
                .Where(field => field.Item1 == "Player")
                .Where(field => field.Item2.Name == "player_id")
                .Count() == 1);
        }
    }
}
