﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharperUniverse.Networking;
using SharperUniverse.Networking.EventArguments;
using SharperUniverse.Utilities;

namespace SharperUniverse.Core
{
    public class SharperInputSystem : BaseSharperSystem<SharperInputComponent>
    {
        public event EventHandler<SharperEntityEventArgs> NewInputEntityCreated;
        public event EventHandler<SharperEntityEventArgs> InputEntityDestroyed;
        
        internal Dictionary<string, Type> CommandBindings { get; set; }

        public SharperInputSystem(GameRunner game) : base(game)
        {
            ComponentUnRegistered += OnInputComponentUnRegistered;
            ComponentRegistered += OnInputComponentRegistered;
        }

        private void OnInputComponentRegistered(object sender, SharperComponentEventArgs e)
        {
            var comp = e.SharperComponent as SharperInputComponent;
            comp.BindingSource.ReceivedMessage += OnInputMessageReceived;
            comp.BindingSource.ClientDisconnected += OnClientDisconnected;
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
            Components.First(x => x.BindingSource.Id == e.Id).Entity.ShouldDestroy = true;
        }

        private void OnInputMessageReceived(object sender, MessageReceivedArgs e)
        {
            var connection = (ISharperConnection)sender;
            var inputComponent = Components.FirstOrDefault(x => x.BindingSource.Id == connection.Id);
            var input = e.Message.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);

            if(!CommandBindings.ContainsKey(input[0]))
            {
                var conn = (ISharperConnection) sender;
                conn.Send($"No command called {input[0]} exists.");
                return;
            }

            var resultType = CommandBindings[input[0]];

            var resultInfo = (IUniverseCommandInfo)Activator.CreateInstance(resultType);
            resultInfo.ProcessArgsAsync(input.SubArray(1, input.Length - 1).ToList());
            inputComponent.CurrentCommand = resultInfo;
        }

        private void OnInputComponentUnRegistered(object sender, SharperComponentEventArgs e)
        {
            InputEntityDestroyed?.Invoke(this, new SharperEntityEventArgs(e.SharperComponent.Entity));
        }

        public override Task CycleUpdateAsync(int deltaMs)
        {
            return Task.CompletedTask;
        }

        public async void OnNewInputConnectionAsync(object sender, NewConnectionArgs e)
        {
            var newEntity = await Game.CreateEntityAsync();
            NewInputEntityCreated?.Invoke(this, new SharperEntityEventArgs(newEntity));
            await RegisterComponentAsync(new SharperInputComponent(newEntity, e.Connection));
        }

        public Task<Dictionary<SharperEntity, IUniverseCommandInfo>> GetEntitiesByCommandInfoTypesAsync(params Type[] commandTypes)
        {
            if (commandTypes.Any(x => !typeof(IUniverseCommandInfo).IsAssignableFrom(x))) throw new SharperTypeViolationException("Unexpected non-command type was found. All types used in GetEntitiesByCommandInfoTypesAsync() must implement IUniverseCommandInfo.");
            var componentsResult = Components.Where(x => x.CurrentCommand != null && commandTypes.Contains(x.CurrentCommand.GetType())).ToList();
            return Task.FromResult(componentsResult.ToDictionary(x => x.Entity, y => y.CurrentCommand));
        }

        public ISharperConnection GetConnectionByEntity(SharperEntity entity)
        {
            return Components.First(x => x.Entity == entity).BindingSource;
        }

        public Task CycleCommandFlushAsync()
        {
            foreach (var inputComponent in Components)
            {
                inputComponent.CurrentCommand = null;
            }
            return Task.CompletedTask;
        }
    }
}
