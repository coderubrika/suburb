using Suburb.Interactables;
using System;
using System.Collections.Generic;

namespace Suburb.Scenarios
{
    public class BaseScenario
    {
        private readonly Dictionary<Type, IInteractable> actors = new();

        public void ApplyActor<T>(T actor)
            where T : IInteractable
        {
            actors[typeof(T)] = actor;
        }
    }
}
