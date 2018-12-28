using System;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	/// <summary>
	///  Interface for defining importing (loading from the database) behaviour for component models. Note - the component must also implement <see cref="IExportable{TModel,TBase}"/>
	/// </summary>
	/// <typeparam name="T">The type of <see cref="BaseSharperComponent"/>'s model it should return when loading from the database.</typeparam>
	public interface IImportable<out T>
	{
		/// <summary>
		/// Defines behaviour for returning a <see cref="BaseSharperComponent"/> from the <typeparam name="T"> model.</typeparam>
		/// </summary>
		/// <param name="entity">The entity this component applies too.</param>
		/// <returns><see cref="BaseSharperComponent"/> model of type <typeparam name="T"></typeparam>.</returns>
		T Import(SharperEntity entity);
		string SystemType { get; }
	}
}