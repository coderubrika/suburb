using Suburb.Screens;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Suburb.UI.Screens
{
    public class MainMenuScreen : BaseScreen
    {
        private ScreensService screensService;

        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button quitButton;

        [Inject]
        public void Construct(ScreensService screensService)
        {
            this.screensService = screensService;

            quitButton.OnClickAsObservable()
                .Subscribe(_ => Application.Quit())
                .AddTo(this);

            Observable.Merge(
                    saveButton.OnClickAsObservable(),
                    loadButton.OnClickAsObservable())
                .Subscribe(_ => screensService.GoTo<SavesScreen>())
                .AddTo(this);

            Observable.Merge(
                    newGameButton.OnClickAsObservable(),
                    continueButton.OnClickAsObservable())
                .Subscribe(_ => screensService.GoTo<GameScreen>())
                .AddTo(this);
        }
    }
}