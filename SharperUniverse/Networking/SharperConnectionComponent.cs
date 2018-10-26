using SharperUniverse.Core;

namespace SharperUniverse.Networking
{
	public class SharperConnectionComponent : BaseSharperComponent
	{
		private SharperEntity _entity;
		public ISharperConnection Connection { get; set; }
		
		public SharperConnectionComponent(SharperEntity entity, ISharperConnection connection) : base(entity)
		{
			_entity = entity;
			Connection = connection;
		}
	}
}