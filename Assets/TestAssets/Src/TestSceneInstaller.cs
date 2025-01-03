using Suburb.Inputs;
using UnityEngine;
using Zenject;

namespace TestAssets.Src
{
    public class TestSceneInstaller : MonoInstaller
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private MainScreen mainScreenPrefab;
        [SerializeField] private Camera camera;
        
        public override void InstallBindings()
        {
            
            // Container.BindInterfacesAndSelfTo<TestAppStartup>()
            //     .AsSingle()
            //     .NonLazy();

            Container.Bind<Camera>().FromComponentInNewPrefab(camera).AsSingle().NonLazy();
            Container.Bind<PlayerController>().FromComponentInNewPrefab(playerController).AsSingle().NonLazy();
            Container.Bind<MainScreen>().FromComponentInNewPrefab(mainScreenPrefab).AsSingle().NonLazy();
        }
    }
}