using System.Linq;
using Suburb.Common;
using Suburb.ResourceMaps;
using Suburb.Screens;
using Suburb.UI.Screens;
using Suburb.Utils;
using UniRx;
using UnityEngine;
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
            Application.targetFrameRate = 0;
            screensService.GoTo<MainMenuScreen>();
        }
    }
}