using FFA.UI;
using Suburb.Inputs;
using Suburb.Screens;
using Suburb.Utils;
using UnityEngine;
using Zenject;

namespace FFA
{
    public class FFAInstaller : MonoInstaller
    {
        [SerializeField] private PlayerView playerView;
        
        private static string PLAYERS_POOL = "PlayersPool";
        
        public override void InstallBindings()
        {
            Container.Bind<InjectCreator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TouchResourceDistributor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MouseResourceDistributor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LayerOrderer>().AsSingle().NonLazy();
            Container.Bind<ScreensFactory>().AsSingle().WithArguments("FFA/Screens").NonLazy();
            Container.Bind<ScreensService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Startup>().AsSingle().NonLazy();
            Container.BindMemoryPool<PlayerView, PlayerView.Pool>()
                .WithInitialSize(4)
                .WithMaxSize(4)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(playerView)
                .UnderTransformGroup(PLAYERS_POOL);
        }
    }
}
