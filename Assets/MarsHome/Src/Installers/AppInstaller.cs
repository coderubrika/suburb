using Suburb.Core;
using UnityEngine;
using Zenject;

namespace Suburb.Installers
{
    public class AppInstaller : MonoInstaller
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private string screensRoot;
        [SerializeField] private string layoutsRoot;
        
        public override void InstallBindings()
        {
            // Container.Bind<ScreensFactory>().AsSingle().WithArguments(screensRoot).NonLazy();
            // Container.Bind<ScreensService>().AsSingle().NonLazy();
            // Container.Bind<LayoutsFactory>().AsSingle().WithArguments(layoutsRoot).NonLazy();
            // Container.BindInterfacesAndSelfTo<LayoutService>().AsSingle().NonLazy();

            // Container.Bind<SavesService>().AsSingle().NonLazy();
            // Container.Bind<LocalizationService>().AsSingle().NonLazy();
            
            // Container.Bind<LocalStorageService>().AsSingle().NonLazy();
            // Container.Bind<WebClientService>().AsSingle().NonLazy();
            
            //Container.Bind<GameStateMachine>().AsSingle();
            //Container.Bind<WorldMapService>().AsSingle();
            //Container.Bind<GameStartup>().AsSingle();
            //Container.Bind<PickDetector>().AsSingle();
            //Container.Bind<WorldCameraController>().AsSingle().WithArguments(playerCamera);
            //Container.BindInterfacesAndSelfTo<ResourcesService>().AsSingle().NonLazy();
            //Container.Bind<ResourceLoader>().AsSingle();

            // Container.BindInterfacesAndSelfTo<MenuSceneService>()
            //     .AsSingle();
            
            Container.BindInterfacesTo<AppStartup>().AsSingle().NonLazy();
        }
    }
}
