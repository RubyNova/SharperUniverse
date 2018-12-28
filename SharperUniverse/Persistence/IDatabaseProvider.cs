using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public interface IDatabaseProvider
	{
		Task<int> Save(List<BaseSharperComponent> components);
		Task Load(int index);
		Task Modify(int index, List<BaseSharperComponent> components);
		Task Delete(int index);
		
		string ConnectionString { set; }
	}
}