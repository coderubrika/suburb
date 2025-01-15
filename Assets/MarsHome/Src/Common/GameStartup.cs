using Suburb.Screens;
using Suburb.UI.Screens;
using UniRx;

namespace Suburb.Common
{
    public class GameStartup
    {
        private readonly ScreensService screensService;
        private readonly MenuSceneService menuSceneService;
        
        public GameStartup(ScreensService screensService, MenuSceneService menuSceneService)
        {
            this.screensService = screensService;
            this.menuSceneService = menuSceneService;
        }
        
        public void NewGame()
        {
            screensService.GoTo<GameScreen>();
            // menuSceneService.AnimateStartup()
            //     .Subscribe(_ =>
            //     {
            //         menuSceneService.Hide();
            //         PlayStartup();
            //     });
        }

        public void ContinueGame()
        {
            
        }

        private void PlayStartup()
        {
            
        }
    }
}