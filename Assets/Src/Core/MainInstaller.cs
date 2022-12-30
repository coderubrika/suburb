using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Suburb.Core
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private string screensPathRoot;

        public override void InstallBindings()
        {
            Container.BindIFactory<string, BaseScreen>().FromFactory<PrefabResourceFactory<BaseScreen>>();
            Container.Bind<ScreensService>()
                .AsSingle()
                .WithArguments(screensPathRoot)
                .NonLazy();
        }
    }
}
