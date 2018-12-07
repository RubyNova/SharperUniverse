using System;
using SharperUniverse.Core;
using SharperUniverse.Persistence;

namespace SharperUniverse.Tests.Stubs
{
	public class FooExportableComponent : BaseSharperComponent, IExportable<FooModel, FooExportableComponent>
	{

		private SharperEntity _entity;
		public string Foo { get; set; }
		
		public FooExportableComponent(SharperEntity entity) : base(entity)
		{
			_entity = entity;
			Foo = "foo";
		}
		
		public FooModel Export()
		{
			return new FooModel() { Foo = Foo };
		}
	}

	public class FooModel : IImportable<FooExportableComponent>
	{
		
		public string Foo { get; set; }
		
		public FooModel(){}
		
		public FooExportableComponent Import(SharperEntity entity)
		{
			return new FooExportableComponent(entity);
		}

		public string SystemType { get; set; } = typeof(FooExportSystem).FullName;
	}
}