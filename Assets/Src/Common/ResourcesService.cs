using System;
using System.Collections.Generic;
using System.Linq;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Zenject;
using Object = UnityEngine.Object;

namespace Suburb.Common
{
    public class ResourcesService : IInitializable
    {
        private readonly InjectCreator injectCreator;
        
        private readonly Dictionary<string, object> pools = new();
        private readonly Dictionary<string, object> poolGroups = new();
        private readonly Dictionary<string, PrefabRef> prefabs = new();
        private readonly Dictionary<string, (GameObject[] Prefabs, object[] Instances)> prefabsInstancesGroups = new();
        
        private Transform root;
        
        public ResourcesService(
            InjectCreator injectCreator,
            PrefabsRepository prefabsRepository,
            ResourcesGroupsRepository resourcesGroupsRepository)
        {
            this.injectCreator = injectCreator;
            
            foreach (var prefab in prefabsRepository.Items)
                prefabs.Add(prefab.name, prefab);

            foreach (var pair in resourcesGroupsRepository.Items)
                prefabsInstancesGroups.Add(pair.Name, (pair.Prefabs, null));
        }
        
        public AssetsPool<TComponent> GetPool<TComponent>(TComponent prototype)
            where TComponent : Component, IPoolItem 
        {
            if (pools.TryGetValue(prototype.name, out object pool))
                return (AssetsPool<TComponent>)pool;

            pool = injectCreator.Create<AssetsPool<TComponent>>(new object[] { prototype, root, prototype.name });

            pools.Add(prototype.name, pool);
            return (AssetsPool<TComponent>)pool;
        }

        public AssetsGroupPool<TComponent> GetPoolGroup<TComponent>(string groupName, TComponent[] prototypes)
            where TComponent : Component, IPoolItem 
        {
            if (poolGroups.TryGetValue(groupName, out object poolGroup))
                return (AssetsGroupPool<TComponent>)poolGroup;
            
            poolGroup = injectCreator.Create<AssetsGroupPool<TComponent>>(new object[] { prototypes });

            poolGroups.Add(groupName, poolGroup);
            return (AssetsGroupPool<TComponent>)poolGroup;
        }

        public PrefabRef GetPrefab(string name)
        {
            return prefabs.TryGetValue(name, out var prefab) ? prefab : null;
        }


        public GameObject[] GetPrefabsGroup(string groupName)
        {
            if (!prefabsInstancesGroups.TryGetValue(groupName, out var group))
                return null;
            
            return group.Prefabs;
        }
        
        public T[] GetInstancesGroup<T>(string groupName)
        {
            if (!prefabsInstancesGroups.TryGetValue(groupName, out var group))
                return null;
            var prefabs = group.Prefabs;
            T[] instances = new T[prefabs.Length];
            for (int i = 0; i < instances.Length; i++)
                instances[i] = injectCreator.Create<T>(prefabs[i], root);

            group.Instances = instances as object[];
            return instances;
        }
        
        public void Initialize()
        {
            root = new GameObject("AssetsRoot").transform;
            Object.DontDestroyOnLoad(root.gameObject);
        }
    }
}