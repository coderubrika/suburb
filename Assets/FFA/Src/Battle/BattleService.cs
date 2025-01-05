using System.Linq;
using FFA.Battle.UI;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace FFA.Battle
{
    public enum BattleSide
    {
        Top, Bottom
    }
    
    public class BattleService
    {
        private readonly InjectCreator injectCreator;
        private readonly LayerOrderer layerOrderer;
        private readonly PlayerView.Pool playersPool;
        
        private readonly BattleSideInfo[] battleSideInfos = new BattleSideInfo[2];
        private readonly CompositeDisposable disposables = new();
        
        private RectTransform battleZone;

        public BattleService(
            InjectCreator injectCreator, 
            LayerOrderer layerOrderer,
            PlayerView.Pool playersPool)
        {
            this.injectCreator = injectCreator;
            this.layerOrderer = layerOrderer;
            this.playersPool = playersPool;
        }
        
        public void SetBattleZone(RectTransform rectTransform) => battleZone = rectTransform;
        
        public string GetName(BattleSide side) => GetBattleSideInfo(side).PlayerName;
        
        public void SetName(BattleSide side, string name) => GetBattleSideInfo(side).PlayerName = name;
        
        public int GetPlayersCount(BattleSide side) => GetBattleSideInfo(side).PlayersCount;
        
        public void SetPlayersCount(BattleSide side, int playersCount) => GetBattleSideInfo(side).PlayersCount = playersCount;

        public void SetupPlayer(BattleSide side, Vector2 position)
        {
            var player = playersPool.Spawn();
            player.transform.SetParent(battleZone);
            player.transform.localScale = Vector3.one;
            player.transform.position = position;
            var playerSession = new RectBasedSession(player.transform as RectTransform);
            playerSession.SetBookResources(true);
            playerSession.SetPreventNext(true);
                    
            var touchCompositor = injectCreator.Create<OneTouchPluginCompositor>();
            playerSession.AddCompositor(touchCompositor)
                .AddTo(disposables);
                    
            var swipeTouchPlugin = injectCreator.Create<OneTouchSwipePlugin>();
            touchCompositor.Link<SwipeMember>(swipeTouchPlugin)
                .AddTo(disposables);

            SwipeMember member = playerSession.GetMember<SwipeMember>();
                    
            member.OnDrag
                .Subscribe(delta => player.transform.position += delta.To3())
                .AddTo(disposables);
                    
            layerOrderer.ConnectFirst(playerSession)
                .AddTo(disposables);
        }
        
        private BattleSideInfo GetBattleSideInfo(BattleSide side)
        {
            int idx = (int)side;
            return battleSideInfos[idx] ?? (battleSideInfos[idx] = new BattleSideInfo());
        }
    }
}