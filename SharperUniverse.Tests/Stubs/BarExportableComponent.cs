using SharperUniverse.Core;
using SharperUniverse.Persistence;

namespace SharperUniverse.Tests.Stubs
{
	public class BarExportableComponent : BaseSharperComponent, IExportable<BarModel, BarExportableComponent>
	{
		public string Foo { get; set; }
		
		public BarExportableComponent(SharperEntity entity) : base(entity)
		{
			Foo = "foo";
		}
		
		public BarModel Export()
		{
			return new BarModel() { Foo = Foo };
		}
	}

	public class BarModel : IImportable<BarExportableComponent>
	{
		
		public string Foo { get; set; }

		public BarModel()
		{
			SystemType = typeof(BarExportSystem).FullName;
		}
		
		public BarExportableComponent Import(SharperEntity entity)
		{
			return new BarExportableComponent(entity);
		}

		public string SystemType { get; }
	}
}