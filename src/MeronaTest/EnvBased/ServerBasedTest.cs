﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Merona;

namespace MeronaTest.EnvBased
{
    public class ServerBasedTest
    {
        protected Server server { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            server = new Server(Config.defaults);

            server.Start();
            server.Wait();

            Console.WriteLine("Initialize");
        }

        [TestCleanup]
        public void Cleanup()
        {
            server.Kill();

            Console.WriteLine("Cleanup");
        }
    }
}
