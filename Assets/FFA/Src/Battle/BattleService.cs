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
        //private readonly InjectCreator injectCreator;
        //private readonly LayerOrderer layerOrderer;
        private readonly PlayerView.Pool playersPool;
        
        private readonly BattleSideInfo[] battleSideInfos = new BattleSideInfo[2];
        //private readonly CompositeDisposable disposables = new();
        
        private RectTransform battleZone;

        public BattleService(
            InjectCreator injectCreator, 
            LayerOrderer layerOrderer,
            PlayerView.Pool playersPool)
        {
            //this.injectCreator = injectCreator;
            //this.layerOrderer = layerOrderer;
            this.playersPool = playersPool;
        }
        
        public void SetBattleZone(RectTransform rectTransform) => battleZone = rectTransform;
        
        public string GetName(BattleSide side) => GetBattleSideInfo(side).PlayerName;
        
        public void SetName(BattleSide side, string name) => GetBattleSideInfo(side).PlayerName = name;
        
        public Color GetColor(BattleSide side) => GetBattleSideInfo(side).SideColor;
        
        public void SetColor(BattleSide side, Color sideColor) => GetBattleSideInfo(side).SideColor = sideColor;
        
        public int GetPlayersCount(BattleSide side) => GetBattleSideInfo(side).PlayersCount;
        
        public void SetPlayersCount(BattleSide side, int playersCount) => GetBattleSideInfo(side).PlayersCount = playersCount;

        public void SetupPlayer(BattleSide side, Vector2 position)
        {
            var player = playersPool.Spawn(side);
            player.transform.SetParent(battleZone);
            player.transform.localScale = Vector3.one;
            player.transform.position = position;
        }
        
        private BattleSideInfo GetBattleSideInfo(BattleSide side)
        {
            int idx = (int)side;
            return battleSideInfos[idx] ?? (battleSideInfos[idx] = new BattleSideInfo());
        }
    }
}