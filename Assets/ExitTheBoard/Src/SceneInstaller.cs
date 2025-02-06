using Suburb.Inputs;
using Suburb.Utils;
using Zenject;

namespace ExitTheBoard
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<InjectCreator>().AsSingle().NonLazy();
            Container.Bind<LayerOrderer>().AsSingle().NonLazy();
            Container.Bind<MouseResourceDistributor>().AsSingle().NonLazy();
            Container.Bind<ScreenRaycaster>().AsSingle().NonLazy();
        }
    }
}