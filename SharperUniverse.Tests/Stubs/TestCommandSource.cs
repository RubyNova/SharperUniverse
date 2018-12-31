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
