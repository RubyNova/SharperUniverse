using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SharperUniverse.Core;
using SharperUniverse.Persistence;
using SharperUniverse.Tests.Stubs;

namespace SharperUniverse.Tests
{
	public class PersistenceTests
	{
		[Test]
		public async Task CanSaveWithoutDuplicates()
		{
			var runner = Substitute.For<IGameRunner>();
			
			var barSystem = new BarExportSystem(runner);
			var fooSystem = new FooExportSystem(runner);

			runner.CreateEntityAsync().Returns(Task.FromResult(new SharperEntity()));
			
			var entity = await runner.CreateEntityAsync();

			
			var fooComponent = new FooExportableComponent(entity);
			var barComponent = new BarExportableComponent(entity);

			await fooSystem.RegisterComponentAsync(fooComponent);
			await barSystem.RegisterComponentAsync(barComponent);
			
			var provider = new LiteDbProvider(new List<ISharperSystem>()
			{
				fooSystem,
				barSystem
			});

			provider.ConnectionString = $"{Path.GetTempPath()}/SU.db";
			
			var id = provider.Save(new List<BaseSharperComponent>()
			{
				fooComponent,
				barComponent
			});

			await provider.Load(id);

			var barComponents = barSystem.GetComponents();
			var fooComponents = fooSystem.GetComponents();
			
			Assert.AreSame(fooComponents.First().Entity, barComponents.First().Entity);

		}
		
	}
}