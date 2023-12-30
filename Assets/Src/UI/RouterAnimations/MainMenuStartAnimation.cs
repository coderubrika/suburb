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
    public class MainMenuStartAnimation
    {
        private readonly MenuSceneService menuSceneService;
        
        private readonly TMP_Text[] texts;
        private readonly RectTransform[] textMasks;
        private readonly float buttonsBlockWidth;
        private readonly CanvasGroup canvasGroup;
        private readonly ValueStartEndAnimationData<float> canvasStartEndAnimationConfig;
            
        private Sequence mainSequence;
        private Sequence textSequence;
        
        public MainMenuStartAnimation(
            MenuSceneService menuSceneService, 
            TextListAnimationData textListAnimationData,
            CanvasGroup canvasGroup,
            ValueStartEndAnimationData<float> canvasStartEndAnimationConfig)
        {
            this.canvasGroup = canvasGroup;
            this.canvasStartEndAnimationConfig = canvasStartEndAnimationConfig;
            this.menuSceneService = menuSceneService;
            texts = textListAnimationData.Texts;
            textMasks = textListAnimationData.TextMasks;
            buttonsBlockWidth = textListAnimationData.ButtonsBlock.rect.width;

            Animate = new ActItem<FromTo>(Invoke, Finally);
        }
        
        public ActItem<FromTo> Animate { get; }

        private void Invoke(FromTo points, Action<FromTo> next)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                text.color = UIUtils.GetNewAlpha(text.color, 0);

                var maskRect = textMasks[i];
                maskRect.offsetMax = maskRect.offsetMax.ChangeX(-buttonsBlockWidth);
            }

            mainSequence = DOTween.Sequence();
            mainSequence.Append(UIUtils.FadeCanvas(canvasGroup, canvasStartEndAnimationConfig));
            mainSequence.AppendCallback(menuSceneService.AnimateEnterFirst);
            
            textSequence = DOTween.Sequence()
                .AppendInterval(1.5f)
                .OnComplete(() => next?.Invoke(points));
            
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

            textSequence.OnKill(() =>
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    var text = texts[i];
                    text.color = UIUtils.GetNewAlpha(text.color, 1);

                    var maskRect = textMasks[i];
                    maskRect.offsetMax = maskRect.offsetMax.ChangeX(0);
                }
            });
        }

        private void Finally()
        {
            mainSequence?.Kill();
            textSequence?.Kill();
        }
    }
}