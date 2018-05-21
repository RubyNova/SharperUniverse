using System;
using System.Collections.Generic;
using System.Text;
using SharperUniverse.Core;

namespace ExampleECSUsageConsole
{
    class TestComponent : BaseSharperComponent
    {
        public bool State { get; set; }

        public TestComponent(SharperEntity entity, bool startState)
        {
            Entity = entity;
            State = startState;
        }
    }
}
