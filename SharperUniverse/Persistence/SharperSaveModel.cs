using System.Collections.Generic;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public class SharperSaveModel
	{
		public int Id { get; set; }
		public Dictionary<SharperEntity, List<IImportable<BaseSharperComponent>>> Data { get; set; }
		
		public SharperSaveModel() {}
	}
}