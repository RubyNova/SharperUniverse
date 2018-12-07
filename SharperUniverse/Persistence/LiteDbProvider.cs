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
		private IGameRunner _runner;
		
		public LiteDbProvider(List<ISharperSystem> systems, IGameRunner runner)
		{
			_systems = systems;
			_runner = runner;
		}
		
		public int Save(List<BaseSharperComponent> components)
		{
			using (var db = new LiteDatabase(ConnectionString))
			{
				
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

				await _runner.FlushEntitesAsync();
				
				var temp = db.GetCollection<SharperSaveModel>("saves").FindById(index);
				foreach (var pair in temp.Data)
				{
					var entity = new SharperEntity();
					foreach (var component in pair.Value)
					{
						var system = _systems.Find(x => x.GetType().FullName == component.SystemType);
						if (system == null) throw new InvalidSaveStateException($"Cannot find System {component.SystemType}");
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

	}
}