using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.Utilities
{
    [TestClass]
    public class PacketMarshalTest
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        class MyPacket : Packet
        {
            [MarshalAs(UnmanagedType.I4)]
            public int foo;
            [MarshalAs(UnmanagedType.I4)]
            public int bar;
        }

        [TestMethod]
        public void MarshalSizeOf()
        {
            Assert.AreEqual(
                Marshal.SizeOf<Packet>() + sizeof(int) * 2,
                Marshal.SizeOf<MyPacket>());
        }

        [TestMethod]
        public void Serialize()
        {
            var packet = new MyPacket();

            packet.foo = 14;
            packet.bar = 44;

            // TODO : endian
            var bytes = packet.Serialize();
            byte[] expected = new byte[] {
                    14,0,0,0, 44,0,0,0
                };
            
            var offset = Marshal.SizeOf<Packet>();
            for (var i = offset; i < bytes.Length; i++)
            {
                Console.Write(bytes[i].ToString() + " ");
            }
            Console.WriteLine();

            for(var i = offset; i < bytes.Length; i++)
            {
                Assert.AreEqual(
                    expected[i - offset],
                    bytes[i]);
            }
        }

        [TestMethod]
        public void Deserialize()
        {
            byte[] source = new byte[] {
                    0,0,0,0, 0,0,0,0, 14,0,0,0, 44,0,0,0
                };

            var packet = Packet.Deserialize<MyPacket>(source);

            Assert.AreEqual(
                14,
                packet.foo);
            Assert.AreEqual(
                44,
                packet.bar);
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        class MyPacket2 : Packet
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String nickname;
            [MarshalAs(UnmanagedType.I4)]
            public int level;
        }

        [TestMethod]
        public void SerializeAndDeserialize()
        {
            var packet = new MyPacket2();

            packet.name = "hello_world";
            packet.nickname = "hello_nickname";
            packet.level = 1441;

            var serialized = packet.Serialize();
            var deserialized = Packet.Deserialize<MyPacket2>(serialized);

            Assert.AreEqual(
                packet.level,
                deserialized.level);
            Assert.AreEqual(
                packet.name,
                deserialized.name);
            Assert.AreEqual(
                packet.nickname,
                deserialized.nickname);
        }
    }
}
