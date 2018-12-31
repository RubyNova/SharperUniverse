using SharperUniverse.Core;

namespace SharperUniverse.Networking
{
	public class SharperConnectionComponent : BaseSharperComponent
	{
		public ISharperConnection Connection { get; set; }
		
		public SharperConnectionComponent(SharperEntity entity, ISharperConnection connection) : base(entity)
		{
			Connection = connection;
		}
	}
}