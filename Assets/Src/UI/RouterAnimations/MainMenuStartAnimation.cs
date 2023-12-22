using System;
using DG.Tweening;
using Suburb.Common;
using Suburb.ExpressRouter;
using Suburb.ResourceMaps;
using Suburb.Utils;
using Suburb.Utils.Serialization;
using TMPro;
using UnityEngine;

namespace Suburb.UI
{
    public class MainMenuStartAnimation : IRouterAnimation<FromTo>
    {
        private readonly MenuSceneService menuSceneService;
        
        private readonly CanvasGroup canvasGroup;
        private readonly TMP_Text[] texts;
        private readonly RectTransform[] textMasks;
        private readonly float buttonsBlockWidth;

        private Sequence cameraSequence;
        private Sequence textSequence;
        
        public MainMenuStartAnimation(
            MenuSceneService menuSceneService, 
            MainMenuScreenResourceMap resourceMap)
        {
            this.menuSceneService = menuSceneService;
            
            canvasGroup = resourceMap.CanvasGroup;
            texts = resourceMap.Texts;
            textMasks = resourceMap.TextMasks;
            buttonsBlockWidth = resourceMap.ButtonsBlock.rect.width;

            Animate = new ActItem<FromTo>(Invoke, Finally);
        }
        
        public ActItem<FromTo> Animate { get; }

        private void Invoke(FromTo points, Action<FromTo> next)
        {
            canvasGroup.alpha = 0;
            menuSceneService.StandCameraToStart();
            
            for (int i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                text.color = UIUtils.GetNewAlpha(text.color, 0);

                var maskRect = textMasks[i];
                maskRect.offsetMax = maskRect.offsetMax.ChangeX(-buttonsBlockWidth);
            }
            
            cameraSequence = DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutBack));
            menuSceneService.BindAnimation(cameraSequence);
            cameraSequence.OnComplete(() => next?.Invoke(points));
            
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

            menuSceneService.StandCameraToEnd();
            canvasGroup.alpha = 1;
        }
    }
}