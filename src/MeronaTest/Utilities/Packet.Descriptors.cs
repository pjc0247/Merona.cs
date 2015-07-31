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

        public class Login : Packet
        {
            [C2S]
            public string id;
            [C2S]
            public string password;

            [S2C]
            public bool result;
        }

        [TestMethod]
        public void KeyOf()
        {
            Assert.AreEqual(
                1,
                Packet.GetKeyFields<QueryPlayer>()
                /* Tuple<String, FieldInfo>
                 *    item1 : 타겟 Model의 클래스 이름
                 *    item2 : Packet의 FieldInfo
                 */
                .Where(field => field.Item1 == "Player")
                .Where(field => field.Item2.Name == "player_id")
                .Count());
        }

        [TestMethod]
        public void C2S()
        {
            Assert.AreEqual(
                2,
                Packet.GetC2SFields<Login>()
                .Count());

            Assert.AreEqual(
                1,
                Packet.GetC2SFields<Login>()
                .Where(field => field.Name == "id")
                .Count());

            Assert.AreEqual(
                1,
                Packet.GetC2SFields<Login>()
                .Where(field => field.Name == "password")
                .Count());
        }
        [TestMethod]
        public void S2C()
        {
            Assert.AreEqual(
                1,
                Packet.GetS2CFields<Login>()
                .Count());

            Assert.AreEqual(
                1,
                Packet.GetS2CFields<Login>()
                .Where(field => field.Name == "result")
                .Count());
        }
    }
}
