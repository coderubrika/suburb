using UnityEngine;
using Zenject;
using Suburb.Screens;

namespace Suburb.UI.Screens
{
    public class StartScreen : BaseScreen
    {
        private ScreensService screensService;

        [Inject]
        private void Construct(ScreensService screensService)
        {
            this.screensService = screensService;

            
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
