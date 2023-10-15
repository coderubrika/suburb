using System;
using DG.Tweening;
using Suburb.Common;
using Suburb.ExpressRouter;
using Suburb.ResourceMaps;
using UnityEngine;

namespace Suburb.UI
{
    public class SavesIntoAnimation : IRouterAnimation<FromTo>
    {
        private readonly CanvasGroup canvasGroup;
        private Tween fadeTween;
        
        public ActItem<FromTo> Animate { get; }
        
        public SavesIntoAnimation(UIAnimationsService uiAnimationsService)
        {
            var map = uiAnimationsService.GetResourceMap<SavesScreenResourceMap>();
            canvasGroup = map.CanvasGroup;

            Animate = new ActItem<FromTo>(Invoke, Finally);
        }
        
        private void Invoke(FromTo points, Action<FromTo> next)
        {
            canvasGroup.alpha = 0;
            
            fadeTween = canvasGroup.DOFade(1, 0.4f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    next.Invoke(points);
                });
        }

        private void Finally()
        {
            fadeTween?.Kill();
            canvasGroup.alpha = 1;
        }
    }
}