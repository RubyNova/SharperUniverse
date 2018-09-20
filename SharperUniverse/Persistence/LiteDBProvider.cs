using System;
using System.Linq;
using LiteDB;

namespace SharperUniverse.Persistence
{
	public class LiteDBProvider : IDatabaseProvider
	{
		
		public string ConnectionString { get; set; }
		private LiteDatabase db;
		
		public SharperSaveState Save(SharperGameStateModel state)
		{
			var id = db.GetCollection<SharperGameStateModel>("saves").Insert(state);
			return SharperSaveState.Full;
		}

		public SharperGameStateModel Load(int saveIdentity)
		{
			return db.GetCollection<SharperGameStateModel>("saves").FindById(saveIdentity);
		}

		public SharperSaveState Modify(int saveIdentity, SharperGameStateModel state)
		{
			db.GetCollection<SharperGameStateModel>("saves").Delete(save => save.Id == saveIdentity);
			db.GetCollection<SharperGameStateModel>("saves").Insert(state);
			return SharperSaveState.Full;
		}

		public SharperSaveState Delete(int saveIdentity)
		{
			db.GetCollection<SharperGameStateModel>("saves").Delete(save => save.Id == saveIdentity);
			return SharperSaveState.Full;
		}

		public void Clear()
		{
			db.DropCollection("saves");
		}

		public void Connect()
		{
			db = new LiteDatabase(ConnectionString);
		}
	}
}