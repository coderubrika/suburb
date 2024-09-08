using Suburb.Utils;
using UnityEngine;
using Zenject;

namespace TestAssets.Src
{
    public class TestAppStartup : IInitializable
    {
        private readonly InjectCreator injectCreator;
        private readonly MainScreen mainScreenPrefab;
        private readonly PlayerController playerControllerPrefab;
        private readonly Camera camera;
        
        private MainScreen mainScreen;
        
        public TestAppStartup(InjectCreator injectCreator, MainScreen mainScreenPrefab, PlayerController playerControllerPrefab, Camera camera)
        {
            this.injectCreator = injectCreator;
            this.mainScreenPrefab = mainScreenPrefab;
            this.playerControllerPrefab = playerControllerPrefab;
            this.camera = camera;
        }
        
        public void Initialize()
        {
        }
    }
}