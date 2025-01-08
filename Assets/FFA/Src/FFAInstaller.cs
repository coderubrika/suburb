using FFA.Battle;
using Suburb.Inputs;
using Suburb.Screens;
using Suburb.Utils;
using UnityEngine;
using Zenject;
using FFA.Battle.UI;


namespace FFA
{
    public class FFAInstaller : MonoInstaller
    {
        [SerializeField] private PlayerView playerView;
        [SerializeField] private PlayerButton playerButton;
        
        private static string PLAYERS_POOL = "PlayersPool";
        private static string PLAYERS_BUTTONS_POOL = "PlayersButtonsPool";
        
        public override void InstallBindings()
        {
            Container.Bind<InjectCreator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TouchResourceDistributor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MouseResourceDistributor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LayerOrderer>().AsSingle().NonLazy();
            Container.Bind<ScreensFactory>().AsSingle().WithArguments("FFA/Screens").NonLazy();
            Container.Bind<ScreensService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BattleService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Startup>().AsSingle().NonLazy();
            Container.BindMemoryPool<PlayerView, PlayerView.Pool>()
                .WithInitialSize(4)
                .WithMaxSize(4)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(playerView)
                .UnderTransformGroup(PLAYERS_POOL);
            
            Container.BindMemoryPool<PlayerButton, PlayerButton.Pool>()
                .WithInitialSize(4)
                .WithMaxSize(4)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(playerButton)
                .UnderTransformGroup(PLAYERS_BUTTONS_POOL);
        }
    }
}
