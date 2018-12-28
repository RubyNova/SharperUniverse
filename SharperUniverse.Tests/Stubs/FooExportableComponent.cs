using SharperUniverse.Core;
using SharperUniverse.Persistence;

namespace SharperUniverse.Tests.Stubs
{
	public class FooExportableComponent : BaseSharperComponent, IExportable<FooModel, FooExportableComponent>
	{

		public string Foo { get; set; }
		
		public FooExportableComponent(SharperEntity entity) : base(entity)
		{
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

		public FooModel()
		{
			SystemType = typeof(FooExportSystem).FullName;
		}
		
		public FooExportableComponent Import(SharperEntity entity)
		{
			return new FooExportableComponent(entity);
		}

		public string SystemType { get; }
	}
}