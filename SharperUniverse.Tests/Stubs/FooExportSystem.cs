using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests.Stubs
{
	public class FooExportSystem : BaseSharperSystem<FooExportableComponent>
	{
		public FooExportSystem(IGameRunner game) : base(game)
		{
		}

		public List<FooExportableComponent> GetComponents()
		{
			return Components;
		}
		
		public override Task CycleUpdateAsync(int deltaMs)
		{
			return Task.CompletedTask;
		}
	}
}