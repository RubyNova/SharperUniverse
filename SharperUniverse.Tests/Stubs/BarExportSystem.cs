using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Tests.Stubs
{
	public class BarExportSystem : BaseSharperSystem<BarExportableComponent>
	{
		public BarExportSystem(IGameRunner game) : base(game)
		{
		}

		public List<BarExportableComponent> GetComponents()
		{
			return Components;
		}
		
		public override Task CycleUpdateAsync(int deltaMs)
		{
			return Task.CompletedTask;
		}
	}
}