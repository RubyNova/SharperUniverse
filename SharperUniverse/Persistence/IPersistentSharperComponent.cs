using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public interface IPersistentSharperComponent<out T> where T : BaseSharperComponent
	{
		T ExportData();
	}
}