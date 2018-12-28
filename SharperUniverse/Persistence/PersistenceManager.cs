using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public class PersistenceManager
	{
		private IDatabaseProvider _provider;

		public PersistenceManager(Type provider, string connectionString, List<ISharperSystem> systems, IGameRunner runner)
		{
			_provider = (IDatabaseProvider)Activator.CreateInstance(provider, systems, runner);
			_provider.ConnectionString = connectionString;
		}

		public Task<int> Save(List<BaseSharperComponent> components)
		{
			return _provider.Save(components);
		}

		public void Load(int saveIndex)
		{
			_provider.Load(saveIndex);
		}

		public void Modify(int saveIndex, List<BaseSharperComponent> components)
		{
			_provider.Modify(saveIndex, components);
		}

		public void Delete(int saveIndex)
		{
			_provider.Delete(saveIndex);
		}
	}
}