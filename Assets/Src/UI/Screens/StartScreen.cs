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
        private InteractablesSelector interactablesSelector;
        private Rover rover;
        private Land land;
        private WorldCameraController worldCameraController;

        [Inject]
        private void Construct(
            InteractablesSelector interactablesSelector,
            Rover rover,
            WorldCameraController worldCameraController,
            Land land)
        {
            this.interactablesSelector = interactablesSelector;
            this.rover = rover;
            this.land = land;
            this.worldCameraController = worldCameraController;
            this.Log($"DPI {Screen.dpi}");
        }

        protected override void Show()
        {
            worldCameraController.Enable();
            rover.Install();
            land.Install();
            interactablesSelector.SetEnableChecks(true);
            base.Show();
        }

        protected override void Hide()
        {
            worldCameraController.Disable();
            rover.Uninstall();
            land.Uninstall();
            interactablesSelector.SetEnableChecks(false);
            base.Hide();
        }
    }
}
