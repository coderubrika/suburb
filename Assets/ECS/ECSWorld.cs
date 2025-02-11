using System;
using System.Collections.Generic;
using System.Linq;

namespace Suburb.ECS
{
    public class ECSWorld
    {
        private readonly HashSet<int> entities = new();
        private readonly Stack<int> removedEntities = new();
        private readonly Dictionary<int, Dictionary<Type, object>> entityComponents = new();
        private readonly FilterObject filterObject;
        
        internal Dictionary<int, HashSet<Type>> EntityComponentTypes { get; } = new();
        internal Dictionary<Type, HashSet<int>> ComponentEntities { get; } = new();

        public ECSWorld()
        {
            filterObject = new(this);
        }
        
        public int CreateEntity()
        {
            int entity = removedEntities.Count > 0
                ? removedEntities.Pop()
                : entities.Count;

            entities.Add(entity);
            entityComponents.Add(entity, new());
            EntityComponentTypes.Add(entity, new());
            return entity;
        }

        private bool RemoveEntity(int entity)
        {
            if (!entities.Remove(entity)) 
                return false;
            
            removedEntities.Push(entity);
            entityComponents.Remove(entity);
            EntityComponentTypes.Remove(entity);
            
            return true;
        }

        public void AddComponent<TComponent>(int entity, TComponent component)
            where TComponent : struct
        {
            Type componentType = typeof(TComponent);

            if (ComponentEntities.TryGetValue(componentType, out var entitiesInComponent))
            {
                entitiesInComponent.Add(entity);
                entityComponents[entity].Add(componentType, component);
                EntityComponentTypes[entity].Add(componentType);
                return;
            }
            
            ComponentEntities.Add(componentType, new() {entity});
            entityComponents[entity].Add(componentType, component);
            EntityComponentTypes[entity].Add(componentType);
        }
        
        public void RemoveComponent<TComponent>(int entity, TComponent component)
            where TComponent : struct
        {
            Type componentType = typeof(TComponent);
            var entitiesInComponent = ComponentEntities[componentType];
            entitiesInComponent.Remove(entity);
            if (entitiesInComponent.Count == 0)
                ComponentEntities.Remove(componentType);

            var componentsInEntity = entityComponents[entity];
            componentsInEntity.Remove(componentType);
            
            if (componentsInEntity.Count == 0)
                RemoveEntity(entity);
        }

        public IEnumerable<TComponent> GetComponents<TComponent>(int[] selectedEntities)
            where TComponent : struct
        {
            Type componentType = typeof(TComponent);
            foreach (var entity in selectedEntities)
                yield return (TComponent)entityComponents[entity][componentType];
        }
        
        public FilterObject Filter()
        {
            filterObject.Clear();
            return filterObject;
        }

        internal Type GetMinComponent(IEnumerable<Type> componentTypes)
        {
            int min = Int32.MaxValue;
            Type componentType = null;
            
            foreach (var candidate in componentTypes)
            {
                int candidateCount = ComponentEntities[candidate].Count;
                if (candidateCount < min)
                {
                    min = candidateCount;
                    componentType = candidate;
                }
            }

            return componentType;
        }
    }
}