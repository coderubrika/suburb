using Suburb.Utils;
using System;
using System.Collections.Generic;

namespace Suburb.Common
{
    public class GameStateMachine
    {
        private readonly SavesService savesService;
        private readonly InjectCreator injectCreator;
        private readonly WorldMapService WorldMapService;

        private readonly Dictionary<Type, IGameState> states = new();

        private IGameState currentState;

        public GameStateMachine(
            SavesService savesService, 
            InjectCreator injectCreator,
            WorldMapService WorldMapService)
        {
            this.savesService = savesService;
            this.injectCreator = injectCreator;
            this.WorldMapService = WorldMapService;
        }

        public void SwitchTo<T>()
            where T : IGameState
        {
            IGameState newGameState = GetOrCreateGameState<T>();
            currentState?.Disable();
            currentState = newGameState;
            currentState.Enable();
        }

        public void Start()
        {
            WorldMapService.Generate();
            WorldMapService.Show();
            SwitchTo<TravelingState>();
        }

        public void Pause()
        {
            WorldMapService.Hide();
            currentState.Disable();
        }

        private IGameState GetOrCreateGameState<T>()
            where T : IGameState
        {
            Type type = typeof(T);
            
            if (states.TryGetValue(type, out IGameState gameState))
                return gameState;

            gameState = injectCreator.Create<T>();
            states[type] = gameState;
            return gameState;
        }
    }
}
