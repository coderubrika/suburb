using FFA.Screens;
using Suburb.Screens;
using UnityEngine;
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
            Application.targetFrameRate = 120;
            screensService.GoTo<MainScreen>();
        }
    }
}