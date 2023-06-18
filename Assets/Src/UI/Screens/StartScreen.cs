using UnityEngine;
using Zenject;
using Suburb.Screens;

namespace Suburb.UI
{
    public class StartScreen : BaseScreen
    {
        private ScreensService screensService;

        [Inject]
        private void Construct(ScreensService screensService)
        {
            this.screensService = screensService;

            Application.targetFrameRate = 120;
        }

        protected override void Show()
        {
            base.Show();

            screensService.GoTo<MainMenuScreen>();
        }

        protected override void Hide()
        {
            base.Hide();
        }
    }
}
