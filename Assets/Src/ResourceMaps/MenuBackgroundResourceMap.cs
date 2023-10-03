using Suburb.Common;
using UnityEngine;

namespace Suburb.ResourceMaps
{
    public class MenuBackgroundResourceMap : IResourceMap
    {
        private readonly GameObject mars;
        private readonly Transform root;
        public GameObject Mars => mars;
        
        public MenuBackgroundResourceMap(GameObject mars)
        {
            root = new GameObject("MenuBackgroundResourceRoot").transform;
            GameObject.DontDestroyOnLoad(root);
            mars.transform.SetParent(root);
            this.mars = mars;
        }
    }
}