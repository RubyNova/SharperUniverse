using System;
using System.Collections.Generic;
using System.Text;
using SharperUniverse.Core;
using SharperUniverse.Networking;

namespace SharperUniverse.Tests
{
    class TestCommandSource
    {
        public ISharperConnection TestConnection { get; }

        public TestCommandSource(ISharperConnection testConnection)
        {
            TestConnection = testConnection;
        }
    }
}
