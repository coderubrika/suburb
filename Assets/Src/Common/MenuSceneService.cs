using UnityEngine;
using Zenject;

namespace Suburb.Common
{
    public class MenuSceneService : IInitializable
    {
        private readonly ResourcesService resourcesService;
        private readonly Camera camera;
        
        private const string ROOT_NAME = "MenuScreneRoot";
        
        private Transform root;
        private Mars mars;
        
        
        public MenuSceneService(
            ResourcesService resourcesService, 
            Camera camera)
        {
            this.resourcesService = resourcesService;
            this.camera = camera;
        }

        public void Show()
        {
            
        }

        public void Hide()
        {
            
        }

        public void AnimateCamera()
        {
            
        }

        public void Initialize()
        {
            mars = resourcesService.GetInstance<Mars>("Mars");
            root = new GameObject(ROOT_NAME).transform;
            Object.DontDestroyOnLoad(root.gameObject);
            mars.transform.SetParent(root);
        }
    }
}