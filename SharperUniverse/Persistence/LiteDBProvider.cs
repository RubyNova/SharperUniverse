using LiteDB;

namespace SharperUniverse.Persistence
{
	public class LiteDBProvider : IDatabaseProvider
	{
		
		public string ConnectionString { get; set; }
		private LiteDatabase db;
		
		public SharperSaveState Save(SharperGameStateModel state)
		{
			state.Index += 1;
			db.GetCollection<SharperGameStateModel>("saves").Insert(state);
			return SharperSaveState.Full;
		}

		public SharperGameStateModel Load(int saveIdentity)
		{
			return db.GetCollection<SharperGameStateModel>().FindOne(save => save.Index == saveIdentity);
		}

		public SharperSaveState Modify(int saveIdentity, SharperGameStateModel state)
		{
			db.GetCollection<SharperGameStateModel>().Delete(save => save.Index == saveIdentity);
			db.GetCollection<SharperGameStateModel>().Insert(state);
			return SharperSaveState.Full;
		}

		public SharperSaveState Delete(int saveIdentity)
		{
			db.GetCollection<SharperGameStateModel>().Delete(save => save.Index == saveIdentity);
			return SharperSaveState.Full;
		}

		public void Connect()
		{
			db = new LiteDatabase(ConnectionString);
		}
	}
}