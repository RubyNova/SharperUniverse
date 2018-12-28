using System;
using System.Collections.Generic;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	
	/// <summary>
	/// A save model to represent the current game state, used by the <see cref="PersistenceManager"/>.
	/// </summary>
	public class SharperSaveModel
	{
		/// <summary>
		/// The Id of the current save.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// The <see cref="Dictionary{TKey,TValue}"/> for saving to the database.
		/// </summary>
		public Dictionary<string, List<IImportable<BaseSharperComponent>>> Data { get; set; }
		
		/// <summary>
		/// The empty ctor for LiteDB to use.
		/// </summary>
		public SharperSaveModel() {}
	}
}