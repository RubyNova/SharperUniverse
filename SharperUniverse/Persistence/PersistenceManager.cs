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

		public void Connect()
		{
			_provider.Connect();
		}

		public SharperSaveState Save(List<BaseSharperComponent> components)
		{
			var model = new SharperGameStateModel(components);
			return _provider.Save(model);
		}

		public List<BaseSharperComponent> Load(int saveIndex)
		{
			return _provider.Load(saveIndex).Components;
		}

		public SharperSaveState Modify(int saveIndex, List<BaseSharperComponent> components)
		{
			var state = new SharperGameStateModel(components);
			return _provider.Modify(saveIndex, state);
		}

		public SharperSaveState Delete(int saveIndex)
		{
			return _provider.Delete(saveIndex);
		}
	}
}