using System;
using DG.Tweening;
using Suburb.Common;
using Suburb.ExpressRouter;
using Suburb.ResourceMaps;
using UnityEngine;

namespace Suburb.UI
{
    public class SavesLeaveAnimation : IRouterAnimation<FromTo>
    {
        private readonly CanvasGroup canvasGroup;
        private Tween fadeTween;
        
        public ActItem<FromTo> Animate { get; }
        
        public SavesLeaveAnimation(UIAnimationsService uiAnimationsService)
        {
            var map = uiAnimationsService.GetResourceMap<SavesScreenResourceMap>();
            canvasGroup = map.CanvasGroup;

            Animate = new ActItem<FromTo>(Invoke, Finally);
        }
        
        private void Invoke(FromTo points, Action<FromTo> next)
        {
            fadeTween = canvasGroup.DOFade(0, 0.4f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    next.Invoke(points);
                });
        }

        private void Finally()
        {
            fadeTween?.Kill();
            canvasGroup.alpha = 0;
        }
    }
}