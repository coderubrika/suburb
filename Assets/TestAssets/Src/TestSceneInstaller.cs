using Suburb.Inputs;
using Suburb.Utils;
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
            Container.Bind<InjectCreator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TouchResourceDistributor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MouseResourceDistributor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LayerOrderer>().AsSingle().NonLazy();
            Container.Bind<Camera>().FromComponentInNewPrefab(camera).AsSingle().NonLazy();
            Container.Bind<PlayerController>().FromComponentInNewPrefab(playerController).AsSingle().NonLazy();
            Container.Bind<MainScreen>().FromComponentInNewPrefab(mainScreenPrefab).AsSingle().NonLazy();
        }
    }
}