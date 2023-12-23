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
        private SavesService savesService;
        private LayoutService layoutService;
        
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button savesButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextListAnimationData resourceMap;
        [SerializeField] private ValueStartEndAnimationData<float> fadeInAnimationData;
        [SerializeField] private ValueStartEndAnimationData<float> fadeOutAnimationData;

        [Inject]
        public void Construct(
            ScreensService screensService,
            SavesService savesService,
            GameStateMachine gameStateMachine,
            LayoutService layoutService,
            PrefabsRepository prefabsRepository,
            InjectCreator injectCreator,
            MenuSceneService menuSceneService)
        {
            this.savesService = savesService;
            
            var startAnimation = injectCreator.Create<MainMenuStartAnimation>(resourceMap, canvasGroup, fadeInAnimationData);
            var intoAnimation = new CompositeAnimation(
                _ =>
                {
                    menuSceneService.AnimateEnter();
                    return UIUtils.FadeCanvas(canvasGroup, fadeInAnimationData);
                });
            var leaveAnimation = new CompositeAnimation(points =>
            {
                if (points.To.Name == nameof(SavesScreen))
                    menuSceneService.AnimateRight();
                return UIUtils.FadeCanvas(canvasGroup, fadeOutAnimationData);
            });
            
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