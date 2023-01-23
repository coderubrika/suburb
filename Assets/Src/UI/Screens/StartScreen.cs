using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suburb.Core;
using Suburb.Selectors;
using UniRx;
using Zenject;
using Suburb.Interactables;
using Suburb.Common;

namespace Suburb.UI
{
    public class StartScreen : BaseScreen
    {
        private InteractablesSelector interactablesSelector;
        private Rover rover;
        private PointerService pointerService;
        private WorldCameraController worldCameraController;

        [Inject]
        private void Construct(
            InteractablesSelector interactablesSelector,
            Rover rover,
            WorldCameraController worldCameraController,
            PointerService pointerService)
        {
            this.interactablesSelector = interactablesSelector;
            this.rover = rover;
            this.worldCameraController = worldCameraController;
        }

        protected override void Show()
        {
            worldCameraController.Enable();
            rover.Install();
            interactablesSelector.SetEnableChecks(true);
            base.Show();
        }

        protected override void Hide()
        {
            worldCameraController.Disable();
            rover.Uninstall();
            interactablesSelector.SetEnableChecks(false);
            base.Hide();
        }
    }
}
