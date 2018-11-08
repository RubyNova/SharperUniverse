using System;
using System.Collections.Generic;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public class PersistenceManager
	{
		internal IDatabaseProvider _provider;

		public PersistenceManager(Type provider, string connectionString, List<ISharperSystem> systems)
		{
			_provider = (IDatabaseProvider)Activator.CreateInstance(provider, systems);
			_provider.ConnectionString = connectionString;
		}

		public int Save(List<BaseSharperComponent> components)
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