using Suburb.Common;
using Suburb.Interactables;
using Suburb.Selectors;
using Suburb.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Suburb.Core
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private string screensPathRoot;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private Rover rover;
        [SerializeField] private SmoothTransitionParam worldCameraControllerParam;
        public override void InstallBindings()
        {
            Container.BindIFactory<string, BaseScreen>().FromFactory<PrefabResourceFactory<BaseScreen>>();
            Container.Bind<ScreensService>()
                .AsSingle()
                .WithArguments(screensPathRoot)
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PointerService>().AsSingle();
            Container.Bind<PlayerCamera>().FromComponentInNewPrefab(playerCamera).AsSingle().NonLazy();
            Container.Bind<InteractionRepository>().AsSingle().NonLazy();
            Container.Bind<InteractablesSelector>().AsSingle().NonLazy();
            Container.Bind<Rover>().FromComponentInNewPrefab(rover).AsSingle();
            Container.BindInterfacesAndSelfTo<WorldCameraController>()
                .AsSingle()
                .WithArguments(worldCameraControllerParam);
        }
    }
}
