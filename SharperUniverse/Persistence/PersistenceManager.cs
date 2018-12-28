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

		public Task Load(int saveIndex)
		{
			return _provider.Load(saveIndex);
		}

		public Task Modify(int saveIndex, List<BaseSharperComponent> components)
		{
			return _provider.Modify(saveIndex, components);
		}

		public Task Delete(int saveIndex)
		{
			return _provider.Delete(saveIndex);
		}
	}
}