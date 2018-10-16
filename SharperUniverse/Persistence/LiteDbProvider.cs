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

		private List<ISharperSystem<BaseSharperComponent>> _systems;
		
		public LiteDbProvider(List<ISharperSystem<BaseSharperComponent>> systems)
		{
			_systems = systems;
		}
		
		public int Save(List<BaseSharperComponent> components)
		{
			using (var db = new LiteDatabase(ConnectionString))
			{
				var componentData = ComponentsToExportables(components);
				
				var temp = new Dictionary<SharperEntity, List<IImportable<BaseSharperComponent>>>();
				
				foreach (var component in components)
				{
					
					var cast = component as IExportable<IImportable<BaseSharperComponent>, object>;

					if (cast == null) continue;

					var data = cast.Export();
					
					if (!temp.ContainsKey(component.Entity))
					{
						temp.Add(component.Entity, new List<IImportable<BaseSharperComponent>>()
						{
							data
						});
					}
					else
					{
						temp[component.Entity].Add(data);
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

		public async Task<> Load(int index)
		{
			using (var db = new LiteDatabase(ConnectionString))
			{
				var temp = db.GetCollection<SharperSaveModel>("saves").FindById(index);
				foreach (KeyValuePair<SharperEntity, List<IImportable<BaseSharperComponent>>> pair in temp.Data)
				{
					foreach (var component in pair.Value)
					{
						var system = _systems.Find(x => x.GetType().GetGenericArguments().First() == component.SystemType);
						if (system == null) continue; //TODO throw here
						await system.RegisterComponentAsync(pair.Key, component.Import(pair.Key));
					}
				}
			}

			return Task.CompletedTask;
		}

		public void Modify(int index, List<BaseSharperComponent> components)
		{
			using (var db = new LiteDatabase(ConnectionString))
			{

				var data = ComponentsToExportables(components);
				
				var model = new SharperSaveModel()
				{
					Data = data
				};
				db.GetCollection<SharperSaveModel>("saves").Update(index, model);
			}
		}

		public void Delete(int index)
		{
			throw new System.NotImplementedException();
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