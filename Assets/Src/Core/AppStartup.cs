using System.Linq;
using Suburb.Common;
using Suburb.ResourceMaps;
using Suburb.Screens;
using Suburb.UI.Screens;
using Suburb.Utils;
using UnityEngine;
using Zenject;

namespace Suburb.Core
{
    public class AppStartup : IInitializable
    {
        private readonly ScreensService screensService;
        private readonly ResourcesRepository resourcesRepository;
        private readonly UIAnimationsService uiAnimationsService;
        private readonly InjectCreator injectCreator;
        public AppStartup(
            ScreensService screensService,
            UIAnimationsService uiAnimationsService,
            ResourcesRepository resourcesRepository,
            InjectCreator injectCreator)
        {
            this.screensService = screensService;
            this.resourcesRepository = resourcesRepository;
            this.uiAnimationsService = uiAnimationsService;
            this.injectCreator = injectCreator;
        }

        public void Initialize()
        {
            GameObject marsPrefab = resourcesRepository.Items
                .FirstOrDefault(obj => obj.name == "Mars");
            var menuBackgroundMap = new MenuBackgroundResourceMap(injectCreator.Create(marsPrefab, null));
            uiAnimationsService.AddResourceMap(menuBackgroundMap);
            Application.targetFrameRate = 0;
            screensService.GoTo<MainMenuScreen>();
        }
    }
}