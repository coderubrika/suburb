using Suburb.Common;
using Suburb.Inputs;
using Suburb.Screens;
using Suburb.Detectors;
using Suburb.Utils;
using UnityEngine;
using Zenject;
using Suburb.Cameras;
using Suburb.Utils.Serialization;
using UnityEngine.Serialization;

namespace Suburb.Installers
{
    public class MainInstaller : MonoInstaller
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
            Container.Bind<InjectCreator>().AsSingle().NonLazy();
            // Container.Bind<LocalStorageService>().AsSingle().NonLazy();
            // Container.Bind<WebClientService>().AsSingle().NonLazy();
            
            //Container.Bind<GameStateMachine>().AsSingle();
            //Container.Bind<WorldMapService>().AsSingle();
            //Container.Bind<GameStartup>().AsSingle();
            
            //Container.BindInterfacesAndSelfTo<TouchInputProvider>().AsSingle().NonLazy();
            //Container.BindInterfacesAndSelfTo<MouseInputProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<KeyboardInputProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TouchProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TouchResourceDistributor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LayerOrderer>().AsSingle().NonLazy();
            //Container.BindInterfacesAndSelfTo<PointerGestureConnector>().AsSingle().NonLazy();

            //Container.Bind<PickDetector>().AsSingle();
            //Container.Bind<WorldCameraController>().AsSingle().WithArguments(playerCamera);
            //Container.BindInterfacesAndSelfTo<ResourcesService>().AsSingle().NonLazy();
            //Container.Bind<ResourceLoader>().AsSingle();

            // Container.BindInterfacesAndSelfTo<MenuSceneService>()
            //     .AsSingle();
        }
    }
}
