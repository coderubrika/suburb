using Suburb.Common;
using Suburb.Screens;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

namespace Suburb.UI.Screens
{
    public class GameScreen : BaseScreen
    {
        private ScreensService screensService;
        private GameStateMachine gameStateMachine;

        [SerializeField] private Button menuButton;

        [Inject]
        public void Construct(
            ScreensService screensService,
            GameStateMachine gameStateMachine)
        {
            this.screensService = screensService;
            this.gameStateMachine = gameStateMachine;

            menuButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    gameStateMachine.Pause();
                    screensService.GoToPrevious<MainMenuScreen>();
                })
                .AddTo(this);
        }

        protected override void Show()
        {
            base.Show();
            //gameStateMachine.Start();
        }
    }
}