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
        [SerializeField] private ValueAnimationData<TransformData> cameraAnimationData;
        
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
            
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<WorldMapService>().AsSingle();

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                Container.Bind<IGestureProvider>().To<TouchGestureProvider>().AsSingle().NonLazy();
            else
                Container.Bind<IGestureProvider>().To<MouseGestureProvider>().AsSingle().NonLazy();

            Container.Bind<PickDetector>().AsSingle();
            Container.Bind<WorldCameraController>().AsSingle().WithArguments(playerCamera);
            Container.BindInterfacesAndSelfTo<ResourcesService>().AsSingle().NonLazy();
            Container.Bind<ResourceLoader>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<MenuSceneService>()
                .AsSingle()
                .WithArguments(cameraAnimationData);
        }
    }
}
