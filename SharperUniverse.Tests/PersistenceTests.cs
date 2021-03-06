using System;
using System.Collections.Generic;
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

			var provider =
				new LiteDbProvider(new List<ISharperSystem> {fooSystem, barSystem}, runner);

			var id = await provider.SaveAsync(new List<BaseSharperComponent>
			{
				fooComponent,
				barComponent
			});

			await provider.LoadAsync(id);

			var barComponents = barSystem.GetComponents();
			var fooComponents = fooSystem.GetComponents();
			
			Assert.AreSame(fooComponents.First().Entity, barComponents.First().Entity);

		}

		[Test]
		public async Task NoExcessMemoryUsage()
		{
			var runner = Substitute.For<IGameRunner>();
			
			var barSystem = new BarExportSystem(runner);
			var fooSystem = new FooExportSystem(runner);

			runner.CreateEntityAsync().Returns(Task.FromResult(new SharperEntity()));

			var components = new List<BaseSharperComponent>();

			for (var i = 0; i < 1000; i++)
			{
				var entity = await runner.CreateEntityAsync();

				var fooComponent = new FooExportableComponent(entity);
				var barComponent = new BarExportableComponent(entity);

				await fooSystem.RegisterComponentAsync(fooComponent);
				await barSystem.RegisterComponentAsync(barComponent);
				
			}

			var provider =
				new LiteDbProvider(new List<ISharperSystem> {fooSystem, barSystem}, runner);


			GC.Collect();
			var memory = GC.GetTotalMemory(false);
			
			for (var i = 0; i < 20; i++)
			{
				var id = await provider.SaveAsync(components);

				await provider.LoadAsync(id);
			}

			GC.Collect();
			
			Assert.Less(GC.GetTotalMemory(false), memory + memory / 20);
		}

		[Test]
		public async Task ThrowsOnMissingSystem()
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

			var provider =
				new LiteDbProvider(new List<ISharperSystem> {fooSystem}, runner);

			var id = await provider.SaveAsync(new List<BaseSharperComponent>()
			{
				fooComponent,
				barComponent
			});

			Assert.ThrowsAsync<InvalidSaveStateException>(() => provider.LoadAsync(id));

		}

		[Test]
		public async Task CanHandleMultipleComponentsOnSameEntity()
		{
			var runner = Substitute.For<IGameRunner>();
			
			var barSystem = new BarExportSystem(runner);
			var fooSystem = new FooExportSystem(runner);

			runner.CreateEntityAsync().Returns(Task.FromResult(new SharperEntity()));
			
			var entity = await runner.CreateEntityAsync();

			var fooComponentA = new FooExportableComponent(entity);
			var barComponentA = new BarExportableComponent(entity);
			var fooComponentB = new FooExportableComponent(entity);
			var barComponentB = new BarExportableComponent(entity);

			await fooSystem.RegisterComponentAsync(fooComponentA);
			await barSystem.RegisterComponentAsync(barComponentA);
			await fooSystem.RegisterComponentAsync(fooComponentB);
			await barSystem.RegisterComponentAsync(barComponentB);

			var provider =
				new LiteDbProvider(new List<ISharperSystem> {fooSystem, barSystem}, runner);

			var id = await provider.SaveAsync(new List<BaseSharperComponent>()
			{
				fooComponentA,
				barComponentA,
				fooComponentB,
				barComponentB
			});

			await provider.LoadAsync(id);

			Assert.IsTrue(fooSystem.EntityHasComponent(fooComponentA, entity));
			Assert.IsTrue(fooSystem.EntityHasComponent(fooComponentB, entity));
			Assert.IsTrue(barSystem.EntityHasComponent(barComponentA, entity));
			Assert.IsTrue(barSystem.EntityHasComponent(barComponentB, entity));
		}

		[Test]
		public async Task CanLoadPartialState()
		{
			var runner = Substitute.For<IGameRunner>();
	
			var fooSystem = new FooExportSystem(runner);

			runner.CreateEntityAsync().Returns(Task.FromResult(new SharperEntity()));
			
			var entity = await runner.CreateEntityAsync();

			
			var fooComponent = new FooExportableComponent(entity);

			await fooSystem.RegisterComponentAsync(fooComponent);

			var provider =
				new LiteDbProvider(new List<ISharperSystem> {fooSystem}, runner);


			var id = await provider.SaveAsync(new List<BaseSharperComponent>()
			{
				fooComponent
			});

			Assert.DoesNotThrowAsync(async () =>
			{
				await provider.LoadPartialStateAsync(id, new List<string>()
				{
					fooComponent.Entity.Id.ToString()
				}, true);
			});
		}

		[Test]
		public async Task CanSavePartialState()
		{
			var runner = Substitute.For<IGameRunner>();
	
			var fooSystem = new FooExportSystem(runner);

			runner.CreateEntityAsync().Returns(Task.FromResult(new SharperEntity()));
			
			var entity = await runner.CreateEntityAsync();

			
			var fooComponent = new FooExportableComponent(entity);

			var barComponent = new BarExportableComponent(entity);
			var barSystem = new BarExportSystem(runner);

			await barSystem.RegisterComponentAsync(barComponent);
			await fooSystem.RegisterComponentAsync(fooComponent);
			
			var provider =
				new LiteDbProvider(new List<ISharperSystem> {fooSystem, barSystem}, runner);

			fooComponent.Foo = "foo";
			
			var id = await provider.SaveAsync(new List<BaseSharperComponent>
			{
				fooComponent
			});

			fooComponent.Foo = "bar";
			
			Assert.DoesNotThrowAsync(async () =>
			{
				await provider.ModifyAsync(id, new List<BaseSharperComponent>
				{
					fooComponent,
					barComponent
				});
			});
			
			Assert.AreEqual(fooSystem.GetComponents().First().Foo, "bar");
			
		}
		
		[Test]
		public async Task CanLoadMultiplePartialState()
		{
			var runner = Substitute.For<IGameRunner>();
	
			var fooSystem = new FooExportSystem(runner);

			runner.CreateEntityAsync().Returns(Task.FromResult(new SharperEntity()));
			
			var entity = await runner.CreateEntityAsync();
			
			var fooComponent = new FooExportableComponent(entity);
			
			await fooSystem.RegisterComponentAsync(fooComponent);
			
			var provider =
				new LiteDbProvider(new List<ISharperSystem> {fooSystem}, runner);


			var id = await provider.SaveAsync(new List<BaseSharperComponent>
			{
				fooComponent
			});

			Assert.DoesNotThrowAsync(async () =>
			{
				await provider.LoadPartialStateAsync(id, new List<string>()
				{
					fooComponent.Entity.Id.ToString()
				}, true);
			});
		}
	}
}