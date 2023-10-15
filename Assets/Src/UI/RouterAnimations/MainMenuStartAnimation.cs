using System;
using DG.Tweening;
using Suburb.Cameras;
using Suburb.Common;
using Suburb.ExpressRouter;
using Suburb.ResourceMaps;
using Suburb.Screens;
using Suburb.Utils;
using Suburb.Utils.Serialization;
using TMPro;
using UnityEngine;

namespace Suburb.UI
{
    public class MainMenuStartAnimation : IRouterAnimation<FromTo>
    {
        private readonly Camera uiCamera;
        private readonly Mars mars;
        private readonly CanvasGroup canvasGroup;
        private readonly TMP_Text[] texts;
        private readonly AnimationSettingsData cameraAnim;
        private readonly TransformData cameraStart;
        private readonly TransformData cameraEnd;
        private readonly RectTransform[] textMasks;
        private readonly float buttonsBlockWidth;

        private Sequence cameraSequence;
        private Sequence textSequence;
        
        public MainMenuStartAnimation(
            UIAnimationsService uiAnimationsService,
            CameraService cameraService)
        {
            uiCamera = cameraService.GetCamera(ScreensService.UI_CAMERA);
            var backgroundMap = uiAnimationsService.GetResourceMap<MenuBackgroundResourceMap>();
            mars = backgroundMap.Mars;
            var uiMap = uiAnimationsService.GetResourceMap<MainMenuScreenResourceMap>();
            canvasGroup = uiMap.CanvasGroup;
            texts = uiMap.Texts;
            cameraAnim = uiMap.CameraAnimSettings;
            cameraStart = uiMap.CameraStart;
            cameraEnd = uiMap.CameraEnd;
            textMasks = uiMap.TextMasks;
            buttonsBlockWidth = uiMap.ButtonsBlock.rect.width;

            Animate = new ActItem<FromTo>(Invoke, Finally);
        }
        
        public ActItem<FromTo> Animate { get; }

        private void Invoke(FromTo points, Action<FromTo> next)
        {
            canvasGroup.alpha = 0;
            uiCamera.transform.position = cameraStart.Position;
            uiCamera.transform.localRotation = Quaternion.Euler(cameraStart.Rotation);

            for (int i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                text.color = UIUtils.GetNewAlpha(text.color, 0);

                var maskRect = textMasks[i];
                maskRect.offsetMax = maskRect.offsetMax.ChangeX(-buttonsBlockWidth);
            }
            
            mars.gameObject.SetActive(true);
            cameraSequence = DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutBack));
            
            cameraSequence
                .Append(uiCamera.transform.DORotate(cameraEnd.Rotation, cameraAnim.Duration).SetEase(cameraAnim.Easing))
                .Join(uiCamera.transform.DOMove(cameraEnd.Position, cameraAnim.Duration).SetEase(cameraAnim.Easing))
                .OnComplete(() => next?.Invoke(points));
            
            textSequence = DOTween.Sequence()
                .AppendInterval(1.5f);
            
            for (int i = texts.Length - 1; i >= 0; i--)
            {
                var text = texts[i];
                textSequence.Join(text.DOFade(1f, 0.4f).SetEase(Ease.OutCirc));

                var maskRect = textMasks[i];
                Tween tween = DOTween.To(
                    () => maskRect.offsetMax.x,
                    x => maskRect.offsetMax = maskRect.offsetMax.ChangeX(x),
                    0f, 0.4f).SetEase(Ease.Flash);
                textSequence.Join(tween);
                textSequence.PrependInterval(0.1f);
            }
        }

        private void Finally()
        {
            cameraSequence?.Kill();
            textSequence?.Kill();
            
            for (int i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                text.color = UIUtils.GetNewAlpha(text.color, 1);

                var maskRect = textMasks[i];
                maskRect.offsetMax = maskRect.offsetMax.ChangeX(0);
            }

            uiCamera.transform.position = cameraEnd.Position;
            uiCamera.transform.localRotation = Quaternion.Euler(cameraEnd.Rotation);
            canvasGroup.alpha = 1;
        }
    }
}