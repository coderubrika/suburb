using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Suburb.UI
{
    public class MainMenuScreen : BaseScreen
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button quitButton;

        [Inject]
        public void Construct()
        {
            quitButton.OnClickAsObservable()
                .Subscribe(_ => Application.Quit())
                .AddTo(this);
        }
    }
}