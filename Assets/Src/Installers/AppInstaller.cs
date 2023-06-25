using Suburb.Core;
using Zenject;

namespace Suburb.Installers
{
    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AppStartup>().AsSingle().NonLazy();
        }
    }
}
