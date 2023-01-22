using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suburb.Core;
using Suburb.Selectors;
using UniRx;
using Zenject;
using Suburb.Interactables;

namespace Suburb.UI
{
    public class StartScreen : BaseScreen
    {
        private InteractablesSelector interactablesSelector;
        private Rover rover;

        [Inject]
        private void Construct(InteractablesSelector interactablesSelector, Rover rover)
        {
            this.interactablesSelector = interactablesSelector;
            this.rover = rover;
        }

        protected override void Show()
        {
            rover.Install();
            interactablesSelector.SetEnableChecks(true);
            base.Show();
        }

        protected override void Hide()
        {
            rover.Uninstall();
            interactablesSelector.SetEnableChecks(false);
            base.Hide();
        }
    }
}
