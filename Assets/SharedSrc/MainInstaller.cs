using Suburb.Inputs;
using Zenject;

namespace Suburb.Installers
{
    public class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<KeyboardInputProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TouchProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MouseProvider>().AsSingle().NonLazy();
        }
    }
}
