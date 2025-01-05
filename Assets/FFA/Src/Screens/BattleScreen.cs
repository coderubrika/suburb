using System;
using DG.Tweening;
using FFA.Battle;
using Suburb.Inputs;
using Suburb.Screens;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using FFA.Battle.UI;

namespace FFA.Screens
{
    public class BattleScreen : BaseScreen
    {
        private PlayerView.Pool playersPool;
        private InjectCreator injectCreator;
        private LayerOrderer layerOrderer;
        private BattleService battleService;
        
        [SerializeField] private BattlePreparationView battlePreparation;
        [SerializeField] private RectTransform battleZone;
        
        private readonly CompositeDisposable disposables = new();
        private BattleController battleController;
        [Inject]
        private void Construct(
            PlayerView.Pool playersPool,
            InjectCreator injectCreator,
            LayerOrderer layerOrderer,
            BattleService battleService)
        {
            this.playersPool = playersPool;
            this.injectCreator = injectCreator;
            this.layerOrderer = layerOrderer;
            this.battleService = battleService;
            
            battleService.SetBattleZone(battleZone);
            battleController = new BattleController(battlePreparation);
        }
        
        protected override void Show()
        {
            battleService.SetName(BattleSide.Bottom, "Player1");
            battleService.SetName(BattleSide.Top, "Player2");
            battleService.SetPlayersCount(BattleSide.Bottom, 2);
            battleService.SetPlayersCount(BattleSide.Top, 2);
            
            base.Show();
            battleController.StartPreparation();
        }

        protected override void Hide()
        {
            disposables.Clear();
        }
    }
}