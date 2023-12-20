using Suburb.Common;
using Suburb.Inputs;
using Suburb.Screens;
using Suburb.Detectors;
using Suburb.Utils;
using UnityEngine;
using Zenject;
using Suburb.Cameras;

namespace Suburb.Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera menuSceneCamera;
        [SerializeField] private string screensRoot;
        [SerializeField] private string layoutsRoot;
        
        public override void InstallBindings()
        {
            Container.Bind<ScreensFactory>().AsSingle().WithArguments(screensRoot).NonLazy();
            Container.Bind<ScreensService>().AsSingle().NonLazy();
            Container.Bind<LayoutsFactory>().AsSingle().WithArguments(layoutsRoot).NonLazy();
            Container.BindInterfacesAndSelfTo<LayoutService>().AsSingle().NonLazy();

            Container.Bind<SavesService>().AsSingle().NonLazy();
            Container.Bind<LocalizationService>().AsSingle().NonLazy();
            Container.Bind<InjectCreator>().AsSingle().NonLazy();
            Container.Bind<LocalStorageService>().AsSingle().NonLazy();
            Container.Bind<WebClientService>().AsSingle().NonLazy();

            Container.Bind<MenuSceneService>().AsSingle();
            Container.Bind<Camera>()
                .FromComponentInNewPrefab(menuSceneCamera)
                .WhenInjectedInto<MenuSceneService>();
            
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<WorldMapService>().AsSingle();

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                Container.Bind<IGestureProvider>().To<TouchGestureProvider>().AsSingle().NonLazy();
            else
                Container.Bind<IGestureProvider>().To<MouseGestureProvider>().AsSingle().NonLazy();

            Container.Bind<PickDetector>().AsSingle();
            Container.Bind<WorldCameraController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ResourcesService>().AsSingle().NonLazy();
            Container.Bind<ResourceLoader>().AsSingle();
        }
    }
}
