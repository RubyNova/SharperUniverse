using System.Collections.Generic;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public class SharperGameStateModel
	{
		public List<BaseSharperComponent> Components { get; }

		public SharperGameStateModel(List<BaseSharperComponent> components)
		{
			Components = components;
		}
	}
}