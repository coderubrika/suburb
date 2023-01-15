using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suburb.Mars;
using Suburb.Core;
using Suburb.Selectors;
using UniRx;
using Zenject;

namespace Suburb.UI
{
    public class StartScreen : BaseScreen
    {
        private InteractablesSelector interactablesSelector;

        [Inject]
        private void Construct(InteractablesSelector interactablesSelector)
        {
            this.interactablesSelector = interactablesSelector;
        }

        protected override void Show()
        {
            

            base.Show();

        }
    }
}
