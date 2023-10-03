using Suburb.Common;
using Suburb.Screens;
using Suburb.UI.Layouts;
using Suburb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Suburb.Cameras;
using Suburb.ResourceMaps;
using TMPro;
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
        private ResourcesRepository resourcesRepository;
        private InjectCreator injectCreator;
        private UIAnimationsService uiAnimationsService;
        
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button savesButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private MainMenuScreenResourceMap resourceMap;
        
        [Inject]
        public void Construct(
            ScreensService screensService,
            SavesService savesService,
            GameStateMachine gameStateMachine,
            LayoutService layoutService,
            ResourcesRepository resourcesRepository,
            InjectCreator injectCreator,
            UIAnimationsService uiAnimationsService)
        {
            this.screensService = screensService;
            this.savesService = savesService;
            this.gameStateMachine = gameStateMachine;
            this.resourcesRepository = resourcesRepository;
            this.injectCreator = injectCreator;
            this.uiAnimationsService = uiAnimationsService;
            
            uiAnimationsService.AddResourceMap(resourceMap);
            uiAnimationsService.AddAnimation<BaseScreen, MainMenuScreen>(
                injectCreator.Create<MainMenuStartAnimation>());
            
            quitButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (!savesService.HasChanges)
                    {
                        Application.Quit();
                        return;
                    }

                    IDisposable responseDisposable = null;
                    responseDisposable = layoutService.Setup<IEnumerable<(string, string)>, string, ModalConfirmCancelLayout>(ModalUtils.HaveSaveChangesCancelInput)
                    .Subscribe(status =>
                    {
                        responseDisposable.Dispose();
                        if (status == ModalConfirmCancelLayout.CANCEL_STATUS)
                            Application.Quit();

                        if (status == ModalConfirmLayout.CONFIRM_STATUS)
                        {
                            SavesScreen screen = screensService.GoTo<SavesScreen>();
                            screen.SwitchToSave();
                        }
                    })
                    .AddTo(this);
                })
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
                    responseDisposable = layoutService.Setup<IEnumerable<(string, string)>, string, ModalConfirmCancelLayout>(ModalUtils.HaveSaveChangesCancelInput)
                    .Subscribe(status =>
                    {
                        responseDisposable.Dispose();
                        if (status == ModalConfirmCancelLayout.CANCEL_STATUS)
                        {
                            gameStateMachine.CloseGame();
                            savesService.CreateNewSave();
                            screensService.GoTo<GameScreen>();
                        }

                        if (status == ModalConfirmLayout.CONFIRM_STATUS)
                        {
                            SavesScreen screen = screensService.GoTo<SavesScreen>();
                            screen.SwitchToSave();
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