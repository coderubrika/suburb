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
        private GameControllerGodObject gameController;
        private ScreensService screensService;

        [SerializeField] private Button menuButton;

        [Inject]
        public void Construct(
            GameControllerGodObject gameController,
            ScreensService screensService)
        {
            this.screensService = screensService;
            this.gameController = gameController;

            menuButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    gameController.Stop();
                    screensService.GoToPrevious();
                })
                .AddTo(this);
        }

        protected override void Show()
        {
            base.Show();
            gameController.Start();
        }
    }
}