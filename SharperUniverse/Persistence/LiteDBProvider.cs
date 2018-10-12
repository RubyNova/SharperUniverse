using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using SharperUniverse.Core;
using SharperUniverse.Utilities;

namespace SharperUniverse.Persistence
{
	public class LiteDBProvider : IDatabaseProvider
	{
		
		public string ConnectionString { get; set; }
		private LiteDatabase db;
		
		public int Save(List<BaseSharperComponent> components)
		{
			var dict = new Dictionary<SharperEntity, List<object>>();
			foreach (var component in components)
			{
				var persistentSharperComponent = component as IPersistentSharperComponent<BaseSharperComponent>;
				if (persistentSharperComponent == null) continue;
				if (dict.ContainsKey(component.Entity))
				{
					var thing = persistentSharperComponent.ExportData();
					dict[component.Entity].Add(thing);
				}
				else
				{
					dict.Add(component.Entity, new List<object>()
					{
						persistentSharperComponent.ExportData()
					});
				}
			}
		}

		public SharperGameStateModel Load(int saveIdentity)
		{
			return db.GetCollection<SharperGameStateModel>("saves").FindById(saveIdentity);
		}

		public void Modify(int saveIdentity,  List<BaseSharperComponent> components)
		{
		}

		public void Delete(int saveIdentity)
		{
			db.GetCollection<SharperGameStateModel>("saves").Delete(save => save.Id == saveIdentity);
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