using Suburb.Common;
using Suburb.Screens;
using Suburb.UI.Layouts;
using Suburb.Utils;
using System;
using System.Collections.Generic;
using Suburb.ExpressRouter;
using Suburb.ResourceMaps;
using Suburb.Utils.Serialization;
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
        
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button savesButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private MainMenuScreenResourceMap resourceMap;
        [SerializeField] private ValueAnimationData<float> fadeInAnimationData;
        [SerializeField] private ValueAnimationData<float> fadeOutAnimationData;
        
        [Inject]
        public void Construct(
            ScreensService screensService,
            SavesService savesService,
            GameStateMachine gameStateMachine,
            LayoutService layoutService,
            ResourcesRepository resourcesRepository,
            InjectCreator injectCreator)
        {
            this.screensService = screensService;
            this.savesService = savesService;
            this.gameStateMachine = gameStateMachine;
            this.resourcesRepository = resourcesRepository;
            this.injectCreator = injectCreator;

            var startAnimation = injectCreator.Create<MainMenuStartAnimation>(resourceMap);
            var intoAnimation = injectCreator.Create<FadeCanvasGroupAnimation>(canvasGroup, fadeInAnimationData);
            var leaveAnimation = injectCreator.Create<FadeCanvasGroupAnimation>(canvasGroup, fadeOutAnimationData);
            
            screensService.UseTransition(
                startAnimation.Animate, 
                Rule.AToB(null, nameof(MainMenuScreen)),
                MiddlewareOrder.To).AddTo(this);
            
            screensService.UseTransition(
                intoAnimation.Animate, 
                new Rule(Selector.All().Exclude(null), Selector.One(nameof(MainMenuScreen))),
                MiddlewareOrder.To).AddTo(this);
            
            screensService.UseTransition(
                leaveAnimation.Animate, 
                Rule.ThisToAll(nameof(MainMenuScreen)),
                MiddlewareOrder.From).AddTo(this);
            
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