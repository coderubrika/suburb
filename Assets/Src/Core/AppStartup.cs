using Suburb.Screens;
using Suburb.UI;
using Suburb.Screens;
using Zenject;

namespace Suburb.Core
{
    public class AppStartup : IInitializable
    {
        private readonly ScreensService screensService;
        public AppStartup(ScreensService screensService)
        {
            this.screensService = screensService;
        }

        public void Initialize()
        {
            screensService.GoTo<StartScreen>();
        }
    }
}