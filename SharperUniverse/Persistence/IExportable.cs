namespace SharperUniverse.Persistence
{
	/// <summary>
	/// Interface for defining exporting (saving to the database) behaviour for components. Note - the <typeparam name="TModel"></typeparam> must also implement <see cref="IImportable{T}"/>
	/// </summary>
	/// <typeparam name="TModel"> The savable model you plan on exporting when saving.</typeparam>
	/// <typeparam name="TBase"> The original component class this applies to.</typeparam>
	public interface IExportable<out TModel, out TBase> where TModel : IImportable<TBase>
	{
		/// <summary>
		/// Defines the behaviour for returning a save-able model for the persistence manager.
		/// </summary>
		/// <returns>A <see cref="TModel"/> specified in the generic parameters.</returns>
		TModel Export();
	}
}