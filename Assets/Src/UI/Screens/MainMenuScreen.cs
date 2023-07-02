using Suburb.Common;
using Suburb.Screens;
using Suburb.UI.Layouts;
using Suburb.Utils;
using System;
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
        private GameStateMachine gameStateMachine;
        private LayoutService layoutService;

        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button savesButton;

        [SerializeField] private Button quitButton;

        [Inject]
        public void Construct(
            ScreensService screensService,
            SavesService savesService,
            GameStateMachine gameStateMachine,
            LayoutService layoutService)
        {
            this.screensService = screensService;
            this.savesService = savesService;
            this.gameStateMachine = gameStateMachine;

            quitButton.OnClickAsObservable()
                .Subscribe(_ => Application.Quit())
                .AddTo(this);

            savesButton.OnClickAsObservable()
                .Subscribe(_ => screensService.GoTo<SavesScreen>())
                .AddTo(this);

            newGameButton.OnClickAsObservable()
                .Subscribe(_ => 
                {
                    if (!savesService.TmpData.IsDataHasChanges)
                    {
                        gameStateMachine.CloseGame();
                        savesService.CreateNewSave();
                        screensService.GoTo<GameScreen>();
                        return;
                    }

                    IDisposable responseDisposable = null;
                    responseDisposable = layoutService.Setup<ModalConfirmInput, ExitStatus, ModalConfirmCancelLayout>(new ModalConfirmInput
                    {
                        HeaderIndex = "Есть несохраненные изменения",
                        BodyIndex = "Хотите сохранить изменения?",
                        CancelIndex = "Нет",
                        ConfirmIndex = "Да"
                    })
                    .Subscribe(status =>
                    {
                        responseDisposable.Dispose();
                        if (status == ExitStatus.Cancel)
                        {
                            gameStateMachine.CloseGame();
                            savesService.CreateNewSave();
                            screensService.GoTo<GameScreen>();
                        }

                        if (status == ExitStatus.Confirm)
                        {
                                // еще не готово, гдесь надо вызвать еще одну модалку,
                                // ту что служит для создания нового сохранения
                                // перед этим надо настроить локализацию и сверстать адаптивные модалки, создать Input типы и параметры по умолчанию
                        }
                    })
                    .AddTo(this);
                })
                .AddTo(this);

            continueButton.OnClickAsObservable()
                .Subscribe(_ => screensService.GoTo<GameScreen>())
                .AddTo(this);
        }

        protected override void Show()
        {
            base.Show();
            continueButton.gameObject.SetActive(savesService.HasSelectedSave);
        }
    }
}