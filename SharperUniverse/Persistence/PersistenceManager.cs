using System;

namespace SharperUniverse.Persistence
{
	public class PersistanceManager
	{
		internal IDatabaseProvider _provider;
		internal string _connectionString;

		public PersistanceManager(Type provider, string connectionString)
		{
			_provider = (IDatabaseProvider)Activator.CreateInstance(provider);
		}

		public void Connect()
		{
			_provider.Connect(_connectionString);
		}
	}
}