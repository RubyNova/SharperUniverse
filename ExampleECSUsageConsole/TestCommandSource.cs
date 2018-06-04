using System;
using System.Collections.Generic;
using System.Text;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class TestCommandSource : IUniverseCommandSource
    {
        public int TestIdentifierThing { get; }

        public TestCommandSource(int testId)
        {
            TestIdentifierThing = testId;
        }

        public bool SourceIsSameAsBindingSource(IUniverseCommandSource bindingSource)
        {
            var source = (TestCommandSource)bindingSource;

            return TestIdentifierThing == source.TestIdentifierThing;
        }
    }
}
