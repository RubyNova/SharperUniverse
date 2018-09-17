using System.Collections.Generic;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	
	/// <summary>
	/// Abstraction layer to interface with a database of choosing.
	/// </summary>
	public interface IDatabaseProvider
	{
		
		/// <summary>
		/// To save a particular state to the database.
		/// </summary>
		/// <param name="saveIdentity"></param>
		/// <param name="state"></param>
		/// <returns>a <see cref="SharperSaveState"/> to indicate the success of saving the current state.</returns>
		SharperSaveState Save(SharperGameStateModel state);

		SharperGameStateModel Load(int saveIdentity);

		SharperSaveState Modify(int saveIdentity, SharperGameStateModel state);

		SharperSaveState Delete(int saveIdentity);

		void Connect();

		string ConnectionString { get; set; }

	}
	
	
}