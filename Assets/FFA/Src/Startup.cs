using FFA.Screens;
using Suburb.Screens;
using Zenject;

namespace FFA
{
    public class Startup : IInitializable
    {
        private readonly ScreensService screensService;

        public Startup(ScreensService screensService)
        {
            this.screensService = screensService;
        }
        
        public void Initialize()
        {
            screensService.GoTo<MainScreen>();
        }
    }
}