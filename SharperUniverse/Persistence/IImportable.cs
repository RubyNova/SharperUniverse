using System;
using SharperUniverse.Core;

namespace SharperUniverse.Persistence
{
	public interface IImportable<out T>
	{
		T Import(SharperEntity entity);
		Type SystemType { get; set; }
	}
}