using System;
using System.Collections.Generic;
using Suburb.Utils;
using UniRx;

namespace FFA.Battle
{
    public class BattleStateMachine
    {
        private readonly BattleStateFactory factory;
        private readonly HashSet<string> allowedTransitions = new();
        
        private BattleState state;

        public ReactiveCommand<Type> OnStateChangeStarted { get; } = new();
        public ReactiveCommand<Type> OnStateChanged { get; } = new();
        
        public BattleStateMachine(BattleStateFactory factory, string[] allowedTransitions)
        {
            this.factory = factory;
            this.allowedTransitions.AddRange(allowedTransitions);
        }

        public bool Checkout<TState>()
            where TState : BattleState
        {
            string previousStateName = state?.GetType().Name ?? string.Empty;
            string currentStateName = typeof(TState).Name;
            if (!allowedTransitions.Contains(previousStateName + currentStateName))
                return false;

            //OnStateChangeStarted.Execute();
            
            TState newState = factory.GetState<TState>();
            return false;
        }
        
        
    }
}