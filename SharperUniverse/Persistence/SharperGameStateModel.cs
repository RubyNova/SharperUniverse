using System.Collections.Generic;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public class SharperGameStateModel
	{
		public Dictionary<SharperEntity, List<IComponentModel<BaseSharperComponent>>> Data { get; set; }
		public int Id { get; set; }
	}
}