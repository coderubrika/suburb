using Suburb.Resources;
using Suburb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Suburb.Common
{
    public class ResourceManager : IInitializable, IDisposable
    {
        private readonly ResourcesRepository resourcesRepository;
        private readonly InjectCreator injectCreator;

        private readonly Transform resourcesRoot;

        private GameObject[] resources;
        private Dictionary<string, GameObject[]> resourceGroups = new();

        public ResourceManager(
            ResourcesRepository resourcesRepository,
            InjectCreator injectCreator)
        {
            this.resourcesRepository = resourcesRepository;
            this.injectCreator = injectCreator;

            resourcesRoot = new GameObject("ResourcesRoot").transform;
        }

        public void Dispose()
        {
            DestroyAll();
        }

        public void Initialize()
        {
            //LoadAll();
        }

        public GameObject[] GetResources()
        {
            return resources;
        }

        /*private void LoadAll()
        {
            DestroyAll();

            resources = resourcesRepository.Items
                .Select(resource =>
                {
                    var newResource = prefabCreator.Create(resource, resourcesRoot);
                    newResource.SetActive(false);
                    return newResource;
                })
                .ToArray();
        }*/

        private void DestroyAll()
        {
            if (resources == null)
                return;

            foreach(var resource in resources)
                GameObject.Destroy(resource);

            resources = null;
        }
    }
}
