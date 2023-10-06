using Suburb.Common;
using UniRx;
using UnityEngine;

namespace Suburb.ResourceMaps
{
    public class MenuBackgroundResourceMap : IResourceMap
    {
        private readonly Transform root;
        private readonly Mars mars;

        public Mars Mars => mars;
        
        public MenuBackgroundResourceMap(ResourcesService resourcesService)
        {
            mars = resourcesService.GetInstance<Mars>("Mars");

            root = new GameObject("MenuBackgroundResourceRoot").transform;
            Object.DontDestroyOnLoad(root.gameObject);
            mars.transform.SetParent(root);
        }
    }
}