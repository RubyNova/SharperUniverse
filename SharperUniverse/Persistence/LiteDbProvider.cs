using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public class LiteDbProvider : IDatabaseProvider
	{
		public string ConnectionString { get; set; }

		private List<ISharperSystem> _systems;
		
		public LiteDbProvider(List<ISharperSystem> systems)
		{
			_systems = systems;
		}
		
		public int Save(List<BaseSharperComponent> components)
		{
			using (var db = new LiteDatabase(ConnectionString))
			{
				var componentData = ComponentsToExportables(components);
				
				var temp = new Dictionary<string, List<IImportable<BaseSharperComponent>>>();
				
				foreach (var component in components)
				{
					if (!(component is IExportable<IImportable<BaseSharperComponent>, object> cast)) continue;

					var data = cast.Export();
					
					if (!temp.ContainsKey(component.Entity.Id.ToString()))
					{
						temp.Add(component.Entity.Id.ToString(), new List<IImportable<BaseSharperComponent>>()
						{
							data
						});
					}
					else
					{
						temp[component.Entity.Id.ToString()].Add(data);
					}
				}
				
				var save = new SharperSaveModel()
				{
					Data = temp
				};
				
				var coll = db.GetCollection<SharperSaveModel>("saves");
				
				return coll.Insert(save);
			}
		}

		public async Task Load(int index)
		{
			using (var db = new LiteDatabase(ConnectionString))
			{
				var temp = db.GetCollection<SharperSaveModel>("saves").FindById(index);
				foreach (KeyValuePair<string, List<IImportable<BaseSharperComponent>>> pair in temp.Data)
				{
					var entity = new SharperEntity();
					foreach (var component in pair.Value)
					{
						var system = _systems.Find(x => x.GetType().FullName == component.SystemType);
						if (system == null) continue; //TODO throw here
						await system.RegisterComponentAsync(component.Import(entity));
					}
				}
			}
		}

		public void Modify(int index, List<BaseSharperComponent> components)
		{
			using (var db = new LiteDatabase(ConnectionString))
			{

				var temp = new Dictionary<string, List<IImportable<BaseSharperComponent>>>();
				
				foreach (var component in components)
				{
					
					var cast = component as IExportable<IImportable<BaseSharperComponent>, object>;

					if (cast == null) continue;

					var data = cast.Export();
					
					if (!temp.ContainsKey(component.Entity.Id.ToString()))
					{
						temp.Add(component.Entity.Id.ToString(), new List<IImportable<BaseSharperComponent>>()
						{
							data
						});
					}
					else
					{
						temp[component.Entity.Id.ToString()].Add(data);
					}
				}
				
				var model = new SharperSaveModel()
				{
					Data = temp
				};
				db.GetCollection<SharperSaveModel>("saves").Update(index, model);
			}
		}

		public void Delete(int index)
		{
			using (var db = new LiteDatabase(ConnectionString))
			{
				db.GetCollection<SharperSaveModel>("saves").Delete(index);
			}
		}

		private List<IImportable<BaseSharperComponent>> ComponentsToExportables(List<BaseSharperComponent> components)
		{
			var temp = new List<IImportable<BaseSharperComponent>>();
			
			foreach (var component in components)
			{
				var cast = component as IExportable<IImportable<BaseSharperComponent>, object>;

				if (cast == null) continue;

				var data = cast.Export();
				
				temp.Add(data);
			}

			return temp;
		}

	}
}