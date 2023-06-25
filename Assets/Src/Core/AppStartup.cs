using Suburb.Resources;
using Suburb.Screens;
using Suburb.UI.Screens;
using UnityEngine;
using Zenject;
using System.Linq;
using Suburb.Utils;
using Suburb.Common;

namespace Suburb.Core
{
    public class AppStartup : IInitializable
    {
        private readonly ScreensService screensService;

        public AppStartup(
            ScreensService screensService)
        {
            this.screensService = screensService;
        }

        public void Initialize()
        {
            Application.targetFrameRate = 120;
            screensService.GoTo<MainMenuScreen>();
        }
    }
}