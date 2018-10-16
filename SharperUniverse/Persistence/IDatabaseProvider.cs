using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public interface IDatabaseProvider
	{
		int Save(List<BaseSharperComponent> components);
		Task Load(int index);
		void Modify(int index, List<BaseSharperComponent> components);
		void Delete(int index);
		
		string ConnectionString { get; set; }
	}
}