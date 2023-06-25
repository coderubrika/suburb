using Suburb.Utils;
using System;
using System.Collections.Generic;

namespace Suburb.Common
{
    public class GameStateMachine
    {
        private readonly SavesService savesService;
        private readonly InjectCreator injectCreator;

        private readonly Dictionary<Type, IGameState> states = new();

        public GameCollectedConfig CurrentConfig { get; private set; }

        private IGameState currentState;

        public GameStateMachine(
            SavesService savesService, 
            InjectCreator injectCreator)
        {
            this.savesService = savesService;
            this.injectCreator = injectCreator;
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
            CurrentConfig = savesService.GetLast();
            SwitchTo<TravelingState>();
        }

        public void Pause()
        {
            savesService.Update(CurrentConfig.Id);
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
