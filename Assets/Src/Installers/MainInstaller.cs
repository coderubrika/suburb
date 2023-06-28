using Suburb.Common;
using Suburb.Core.Inputs;
using Suburb.Screens;
using Suburb.Detectors;
using Suburb.Utils;
using UnityEngine;
using Zenject;

namespace Suburb.Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private string uiRoot;
        public override void InstallBindings()
        {
            Container.BindIFactory<string, BaseScreen>().FromFactory<PrefabResourceFactory<BaseScreen>>();
            
            Container.Bind<ScreensService>()
                .AsSingle()
                .WithArguments(uiRoot)
                .NonLazy();

            Container.Bind<SavesService>().AsSingle().NonLazy();
            Container.Bind<InjectCreator>().AsSingle().NonLazy();
            Container.Bind<LocalStorageService>().AsSingle().NonLazy();
            Container.Bind<WebClientService>().AsSingle().NonLazy();

            Container.Bind<PlayerCamera>().FromComponentInNewPrefab(playerCamera).AsSingle().NonLazy();
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<WorldMapService>().AsSingle();

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                Container.Bind<TouchGestureProvider>().AsSingle();
            else
                Container.Bind<MouseGestureProvider>().AsSingle();

            Container.Bind<PickDetector>().AsSingle();
            Container.Bind<WorldCameraController>().AsSingle();
        }
    }
}
