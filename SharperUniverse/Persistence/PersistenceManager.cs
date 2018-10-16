using System;
using System.Collections.Generic;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public class PersistenceManager
	{
		internal IDatabaseProvider _provider;

		public PersistenceManager(Type provider, string connectionString)
		{
			_provider = (IDatabaseProvider)Activator.CreateInstance(provider);
			_provider.ConnectionString = connectionString;
		}

		public int Save()
		{
			foreach(var component)
		}

		public List<BaseSharperComponent> Load(int saveIndex)
		{
			var loaded = _provider.Load(saveIndex);
			Console.WriteLine(loaded);
			return loaded.Components;
		}

		public void Modify(int saveIndex, List<BaseSharperComponent> components)
		{
			var state = new SharperGameStateModel(components);
			_provider.Modify(saveIndex, state);
		}

		public void Delete(int saveIndex)
		{
			_provider.Delete(saveIndex);
		}

		public void Clear()
		{
			_provider.Clear();
		}
	}
}