namespace SharperUniverse.Persistence
{
	public interface IExportable<out TModel, out TBase> where TModel : IImportable<TBase>
	{
		TModel Export();
	}
}