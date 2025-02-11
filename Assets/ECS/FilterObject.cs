using System;
using System.Collections.Generic;
using System.Linq;

namespace Suburb.ECS
{
    public class FilterObject
    {
        private readonly ECSWorld ecsWorld;
        private readonly HashSet<Type> componentTypes = new();
        internal FilterObject(ECSWorld ecsWorld)
        {
            this.ecsWorld = ecsWorld;
        }
        
        public FilterObject Include<TComponent>()
            where TComponent : struct
        {
            componentTypes.Add(typeof(TComponent));
            return this;
        }

        public IEnumerable<int> End()
        {
            Type firstComponentType = ecsWorld.GetMinComponent(componentTypes);
            componentTypes.Remove(firstComponentType);
            var entities = ecsWorld.ComponentEntities[firstComponentType];
            foreach (var entity in entities)
            {
                if (componentTypes.All(ct => ecsWorld.EntityComponentTypes[entity].Contains(ct)))
                    yield return entity;
            }
        }

        internal void Clear()
        {
            componentTypes.Clear();
        }
    }
}