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
        private readonly UIAnimationsService uiAnimationsService;
        private readonly InjectCreator injectCreator;
        public AppStartup(
            ScreensService screensService,
            UIAnimationsService uiAnimationsService,
            InjectCreator injectCreator)
        {
            this.screensService = screensService;
            this.uiAnimationsService = uiAnimationsService;
            this.injectCreator = injectCreator;
        }

        public void Initialize()
        {
            var menuBackgroundMap = injectCreator.Create<MenuBackgroundResourceMap>();
            uiAnimationsService.AddResourceMap(menuBackgroundMap);
            Application.targetFrameRate = 0;
            screensService.GoTo<MainMenuScreen>();
        }
    }
}