using System;
using SharperUniverse.Core;
using SharperUniverse.Persistence;

namespace SharperUniverse.Tests.Stubs
{
	public class BarExportableComponent : BaseSharperComponent, IExportable<BarModel, BarExportableComponent>
	{

		private SharperEntity _entity;
		public string Foo { get; set; }
		
		public BarExportableComponent(SharperEntity entity) : base(entity)
		{
			_entity = entity;
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
		
		public BarModel(){}
		
		public BarExportableComponent Import(SharperEntity entity)
		{
			return new BarExportableComponent(entity);
		}

		public string SystemType { get; set; } = typeof(BarExportSystem).FullName;
	}
}