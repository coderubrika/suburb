using Suburb.Common;
using Suburb.Core.Inputs;
using Suburb.Interactables;
using Suburb.Scenarios;
using Suburb.Screens;
using Suburb.Detectors;
using Suburb.Utils;
using UnityEngine;
using Zenject;

namespace Suburb.Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private string screensPathRoot;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private Rover rover;
        [SerializeField] private Land land;
        [SerializeField] private SmoothTransitionParam worldCameraControllerParam;
        public override void InstallBindings()
        {
            Container.BindIFactory<string, BaseScreen>().FromFactory<PrefabResourceFactory<BaseScreen>>();
            Container.Bind<ScreensService>()
                .AsSingle()
                .WithArguments(screensPathRoot)
                .NonLazy();

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                Container.BindInterfacesAndSelfTo<TouchGestureProvider>().AsSingle();
            else
                Container.BindInterfacesAndSelfTo<MouseGestureProvider>().AsSingle();

            Container.Bind<PlayerCamera>().FromComponentInNewPrefab(playerCamera).AsSingle().NonLazy();
            Container.Bind<InjectCreator>().AsSingle().NonLazy();
            Container.Bind<PickDetector>().AsSingle().NonLazy();
            Container.Bind<GameStateMachine>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WorldCameraController>()
                .AsSingle()
                .WithArguments(worldCameraControllerParam);

            Container.Bind<RoverScenario>().AsSingle();
        }
    }
}
