using FFA.Battle;
using Suburb.Inputs;
using Suburb.Screens;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using Zenject;
using FFA.Battle.UI;

namespace FFA.Screens
{
    public class BattleScreen : BaseScreen
    {
        private BattleService battleService;
        
        [SerializeField] private BattlePreparationView battlePreparation;
        [SerializeField] private BattleFightView battleFight;
        [SerializeField] private BattleFinalView finalView;
        [SerializeField] private RectTransform battleZone;
        
        private readonly CompositeDisposable disposables = new();
        private BattleController battleController;
        [Inject]
        private void Construct(
            InjectCreator injectCreator,
            BattleService battleService,
            ScreensService screensService)
        {
            this.battleService = battleService;
            
            battleService.SetBattleZone(battleZone);
            battleController = injectCreator.Create<BattleController>(battlePreparation, battleFight);
            battleController.OnBack
                .Subscribe(_ => screensService.GoToPrevious())
                .AddTo(this);
        }
        
        protected override void Show()
        {
            battleService.SetName(BattleSide.Bottom, "Player1");
            battleService.SetName(BattleSide.Top, "Player2");
            battleService.SetPlayersCount(BattleSide.Bottom, 2);
            battleService.SetPlayersCount(BattleSide.Top, 2);
            battleService.SetColor(BattleSide.Bottom, Color.blue);
            battleService.SetColor(BattleSide.Top, Color.red);
            
            base.Show();
            battleController.StartPreparation();
        }

        protected override void Hide()
        {
            battleController.Clear();
            disposables.Clear();
        }
    }
}