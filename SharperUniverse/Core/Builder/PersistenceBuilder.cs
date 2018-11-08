using System;
using SharperUniverse.Persistence;

namespace SharperUniverse.Core.Builder
{
	public class PersistenceBuilder
	{
		private readonly GameRunner _game;
		private readonly Type _provider;
		
		public PersistenceBuilder(GameRunner game, Type provider)
		{
			_game = game;
			_provider = provider;
		}

		public OptionsBuilder WithConnectionString(string connectionString)
		{
			_game.PeristenceManager = new PersistenceManager(_provider, connectionString, _game.Systems);
			return new OptionsBuilder(_game);
		}
	}
}