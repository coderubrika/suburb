using System.Collections.Generic;
using FFA.Battle.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FFA.Battle
{
    public enum BattleSide
    {
        Top, Bottom
    }
    
    public class BattleService
    {
        private readonly PlayerView.Pool playersPool;
        
        private readonly BattleSideInfo[] battleSideInfos = new BattleSideInfo[2];
        private readonly List<PlayerView> topPlayers = new();
        private readonly List<PlayerView> bottomPlayers = new();
        
        private RectTransform battleZone;

        public RectTransform BattleZone => battleZone;
        
        public BattleService(PlayerView.Pool playersPool)
        {
            this.playersPool = playersPool;
        }
        
        public void SetBattleZone(RectTransform rectTransform) => battleZone = rectTransform;
        
        public string GetName(BattleSide side) => GetBattleSideInfo(side).PlayerName;
        
        public void SetName(BattleSide side, string name) => GetBattleSideInfo(side).PlayerName = name;
        
        public Color GetColor(BattleSide side) => GetBattleSideInfo(side).SideColor;
        
        public void SetColor(BattleSide side, Color sideColor) => GetBattleSideInfo(side).SideColor = sideColor;
        
        public int GetPlayersCount(BattleSide side) => GetBattleSideInfo(side).PlayersCount;
        
        public void SetPlayersCount(BattleSide side, int playersCount) => GetBattleSideInfo(side).PlayersCount = playersCount;

        public PlayerView CreatePlayer(BattleSide side, Vector2 position)
        {
            PlayerData playerData = new PlayerData
            {
                BodyColor = Random.ColorHSV(0.7f, 1f, 0.7f, 1f, 0, 1f),
                BodyBorderColor = Random.ColorHSV(0f, 0.7f, 0.7f, 1f, 0, 1f),
                BackgroundColor = GetColor(side),
            };
            var player = playersPool.Spawn(side, playerData);
            player.transform.SetParent(battleZone);
            player.transform.localScale = Vector3.one;
            player.transform.position = position;
            
            GetPlayersList(side).Add(player);
            
            return player;
        }

        public void DeletePlayer(PlayerView player)
        {
            if (GetPlayersList(player.Side).Remove(player))
                playersPool.Despawn(player);
        }
        
        private BattleSideInfo GetBattleSideInfo(BattleSide side)
        {
            int idx = (int)side;
            return battleSideInfos[idx] ?? (battleSideInfos[idx] = new BattleSideInfo());
        }
        
        public List<PlayerView> GetPlayersList(BattleSide side) => side == BattleSide.Top ? topPlayers : bottomPlayers;
    }
}