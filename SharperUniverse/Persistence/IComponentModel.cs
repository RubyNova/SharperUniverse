using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public interface IComponentModel<T> where T : BaseSharperComponent
	{
		BaseSharperSystem<T> System { get; set; }

		T ImportData();
	}
}