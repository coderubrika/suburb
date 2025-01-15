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
        private readonly Dictionary<string, PrefabRef> prefabs = new();
        
        private Transform root;
        
        public ResourcesService(PrefabsRepository prefabsRepository)
        {
            foreach (var prefab in prefabsRepository.Items)
                prefabs.Add(prefab.name, prefab);
        }

        public PrefabRef GetPrefab(string name)
        {
            return prefabs.TryGetValue(name, out var prefab) ? prefab : null;
        }
        
        public void Initialize()
        {
            root = new GameObject("AssetsRoot").transform;
            Object.DontDestroyOnLoad(root.gameObject);
        }
    }
}