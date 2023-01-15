using Suburb.Common;
using Suburb.Selectors;
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

        public override void InstallBindings()
        {
            Container.BindIFactory<string, BaseScreen>().FromFactory<PrefabResourceFactory<BaseScreen>>();
            Container.Bind<ScreensService>()
                .AsSingle()
                .WithArguments(screensPathRoot)
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PointerService>().AsSingle();
            Container.Bind<PlayerCamera>().FromComponentInNewPrefab(playerCamera).AsSingle().NonLazy();
            Container.Bind<InteractablesSelector>().AsSingle();
        }
    }
}
