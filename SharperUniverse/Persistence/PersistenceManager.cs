using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	/// <summary>
	/// The class that provides interactions between the database of choice.
	/// </summary>
	public class PersistenceManager
	{
		private IDatabaseProvider _provider;

		/// <param name="provider">The typeof <see cref="IDatabaseProvider"/> to use.</param>
		/// <param name="connectionString">The string for connecting to the database.</param>
		/// <param name="systems">The systems in the current <see cref="IGameRunner"/>.</param>
		/// <param name="runner">The <see cref="IGameRunner"/> for this game.</param>
		public PersistenceManager(Type provider, string connectionString, List<ISharperSystem> systems, IGameRunner runner)
		{
			_provider = (IDatabaseProvider)Activator.CreateInstance(provider, systems, runner);
			_provider.ConnectionString = connectionString;
		}


		///<see cref="IDatabaseProvider.Save(List{SharperUniverse.Core.BaseSharperComponent})"/>
		public Task<int> SaveAsync(List<BaseSharperComponent> components)
		{
			return _provider.SaveAsync(components);
		}

		///<see cref="IDatabaseProvider.Load(int)"/>
		public Task LoadAsync(int saveIndex)
		{
			return _provider.LoadAsync(saveIndex);
		}
		
		///<see cref="IDatabaseProvider.Modify(int, List{BaseSharperComponent})"/>
		public Task ModifyAsync(int saveIndex, List<BaseSharperComponent> components)
		{
			return _provider.ModifyAsync(saveIndex, components);
		}

		///<see cref="IDatabaseProvider.Delete(int)"/>
		public Task DeleteAsync(int saveIndex)
		{
			return _provider.DeleteAsync(saveIndex);
		}
	}
}