using Suburb.Common;
using Suburb.Screens;
using Suburb.UI.Layouts;
using Suburb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Suburb.Cameras;
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
        private CameraService cameraService;
        private ResourcesRepository resourcesRepository;
        private InjectCreator injectCreator;

        [SerializeField] private RectTransform buttonsBlock;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button savesButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Vector3 cameraStartRotation;
        [SerializeField] private Vector3 cameraEndRotation;
        [SerializeField] private Vector3 cameraStartPosition;
        [SerializeField] private Vector3 cameraEndPosition;
        [SerializeField] private float cameraRotateDuration;
        [SerializeField] private Ease cameraRotateEasing;
        [SerializeField] private float cameraMoveDuration;
        [SerializeField] private Ease cameraMoveEasing;
        [SerializeField] private TMP_Text[] texts;
        [SerializeField] private RectTransform[] textMasks;
        
        private GameObject marsPlanet;
        private Camera uiCamera;
        private float buttonsBlockWidth;
        
        [Inject]
        public void Construct(
            ScreensService screensService,
            SavesService savesService,
            GameStateMachine gameStateMachine,
            LayoutService layoutService,
            CameraService cameraService,
            ResourcesRepository resourcesRepository,
            InjectCreator injectCreator)
        {
            this.screensService = screensService;
            this.savesService = savesService;
            this.gameStateMachine = gameStateMachine;
            this.cameraService = cameraService;
            this.resourcesRepository = resourcesRepository;
            this.injectCreator = injectCreator;

            uiCamera = cameraService.GetCamera(ScreensService.UI_CAMERA);
            buttonsBlockWidth = buttonsBlock.rect.width;
            
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
            StartAnimation();
            continueButton.gameObject.SetActive(savesService.HasSelectedSave);
        }

        private void StartAnimation()
        {
            canvasGroup.alpha = 0;
            uiCamera.transform.localRotation = Quaternion.Euler(cameraStartRotation);
            uiCamera.transform.position = cameraStartPosition;
            
            if (marsPlanet == null)
            {
                var marsPlanetPrefab = resourcesRepository.Items.FirstOrDefault(item => item.name == "Mars");
                marsPlanet = injectCreator.Create(marsPlanetPrefab, null);
            }

            for (int i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                text.color = UIUtils.GetNewAlpha(text.color, 0);

                var maskRect = textMasks[i];
                maskRect.offsetMax = maskRect.offsetMax.ChangeX(-buttonsBlockWidth);
            }
                
            
            marsPlanet.SetActive(true);
            Sequence sequence = DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutBack));
            
            sequence
                .Append(uiCamera.transform.DORotate(cameraEndRotation, cameraRotateDuration).SetEase(cameraRotateEasing))
                .Join(uiCamera.transform.DOMove(cameraEndPosition, cameraMoveDuration).SetEase(cameraMoveEasing));

            Sequence textsSequence = DOTween.Sequence()
                .AppendInterval(1.5f);
            
            for (int i = texts.Length - 1; i >= 0; i--)
            {
                float intervalPiece = i * 10f;
                var text = texts[i];
                textsSequence.Join(text.DOFade(1f, 0.4f).SetEase(Ease.OutCirc));

                var maskRect = textMasks[i];
                Tween tween = DOTween.To(
                    () => maskRect.offsetMax.x,
                    x => maskRect.offsetMax = maskRect.offsetMax.ChangeX(x),
                    0f, 0.4f).SetEase(Ease.Flash);
                textsSequence.Join(tween);
                textsSequence.PrependInterval(0.1f);
            }

        }
    }
}