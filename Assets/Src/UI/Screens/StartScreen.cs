using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suburb.Core;
using Suburb.Selectors;
using UniRx;
using Zenject;
using Suburb.Interactables;
using Suburb.Common;
using Suburb.Utils;

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
