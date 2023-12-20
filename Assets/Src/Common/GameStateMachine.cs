using Suburb.Utils;
using System;
using System.Collections.Generic;
using Suburb.Cameras;
using UniRx;
using UnityEngine;

namespace Suburb.Common
{
    public class GameStateMachine
    {
        private readonly InjectCreator injectCreator;
        private readonly WorldMapService WorldMapService;
        
        private readonly Dictionary<Type, IGameState> states = new();

        private IGameState currentState;
        private bool isGameplayInited;

        public GameStateMachine(
            InjectCreator injectCreator,
            WorldMapService WorldMapService)
        {
            this.injectCreator = injectCreator;
            this.WorldMapService = WorldMapService;
        }

        public void SwitchTo<T>()
            where T : IGameState
        {
            IGameState newGameState = GetOrCreateGameState<T>();

            if (newGameState == currentState)
                return;

            currentState?.Disable();
            currentState = newGameState;
            currentState.Enable();
        }

        public void Start()
        {
            if (!isGameplayInited)
            {
                WorldMapService.Generate();
                isGameplayInited = true;
                SwitchTo<TravelingState>();
            }

            currentState.Continue();
            WorldMapService.Show();
        }

        public void Pause()
        {
            WorldMapService.Hide();
            currentState.Pause();
        }

        public void CloseGame()
        {
            isGameplayInited = false;
            currentState?.Disable();
            currentState = null;
            WorldMapService.Clear();
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
