using System.Collections.Generic;
using System.Threading.Tasks;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	/// <summary>
	/// The interface for writing DatabaseProviders like the built in <see cref="LiteDbProvider"/>.
	/// </summary>
	public interface IDatabaseProvider
	{
		/// <summary>
		/// The method for saving a <see cref="List{BaseSharperComponent}"/> to the provider.
		/// </summary>
		/// <param name="components"> the <see cref="BaseSharperComponent"/>s you want to save.</param>
		/// <returns>a <see cref="Task{Int}"/> indicating the save index.</returns>
		Task<int> SaveAsync(List<BaseSharperComponent> components);
		
		/// <summary>
		/// Loads a state from the database.
		/// </summary>
		/// <param name="index"> The index of the save you want to load.</param>
		/// <returns>A <see cref="Task"/></returns>
		Task LoadAsync(int index);
		
		/// <summary>
		/// Saves a partial state to the database.
		/// </summary>
		/// <param name="index"> The index of the save state you want to modify.</param>
		/// <param name="components"> The <see cref="BaseSharperComponent"/>s you want to save.</param>
		/// <returns>A <see cref="Task"/></returns>
		Task ModifyAsync(int index, List<BaseSharperComponent> components);
		
		/// <summary>
		/// Deletes a save state from the database.
		/// </summary>
		/// <param name="index">The index of the save state you want to delete.</param>
		/// <returns>A <see cref="Task"/></returns>
		Task DeleteAsync(int index);
		
		/// <summary>
		/// The string used for connecting to the database.
		/// </summary>
		string ConnectionString { set; }
	}
}