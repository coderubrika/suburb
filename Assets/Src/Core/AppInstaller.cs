using UnityEngine;
using Zenject;

namespace Suburb.Core
{
    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AppStartup>().AsSingle().NonLazy();
        }
    }
}
