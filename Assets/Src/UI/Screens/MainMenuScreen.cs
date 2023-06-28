using Suburb.Common;
using Suburb.Screens;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Suburb.UI.Screens
{
    public class MainMenuScreen : BaseScreen
    {
        private ScreensService screensService;
        private SavesService savesService;

        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button quitButton;

        [Inject]
        public void Construct(
            ScreensService screensService,
            SavesService savesService)
        {
            this.screensService = screensService;
            this.savesService = savesService;

            quitButton.OnClickAsObservable()
                .Subscribe(_ => Application.Quit())
                .AddTo(this);

            Observable.Merge(
                    saveButton.OnClickAsObservable(),
                    loadButton.OnClickAsObservable())
                .Subscribe(_ => screensService.GoTo<SavesScreen>())
                .AddTo(this);

            newGameButton.OnClickAsObservable()
                .Subscribe(_ => 
                {
                    savesService.Create();
                    screensService.GoTo<GameScreen>();
                })
                .AddTo(this);

            continueButton.OnClickAsObservable()
                .Subscribe(_ => screensService.GoTo<GameScreen>())
                .AddTo(this);
        }

        protected override void Show()
        {
            base.Show();
            continueButton.gameObject.SetActive(savesService.SelectedData != null);
        }
    }
}