using Suburb.Screens;
using UnityEngine;
using Zenject;

namespace FFA
{
    public class FFAInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ScreensFactory>().AsSingle().WithArguments("FFA/Screens").NonLazy();
            Container.Bind<ScreensService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Startup>().AsSingle().NonLazy();
        }
    }
}
