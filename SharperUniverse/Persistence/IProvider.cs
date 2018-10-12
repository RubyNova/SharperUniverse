using System.Collections.Generic;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public interface IDatabaseProvider
	{
		int Save(List<BaseSharperComponent> components);

		SharperGameStateModel Load(int saveIdentity);

		void Modify(int saveIdentity, List<BaseSharperComponent> components);

		void Delete(int saveIdentity);

		void Connect();

		void Clear();
		
		string ConnectionString { get; set; }

	}
	
	
}