using System;
using DG.Tweening;
using Suburb.Common;
using Suburb.ExpressRouter;
using Suburb.ResourceMaps;
using UnityEngine;

namespace Suburb.UI
{
    public class MainMenuLeaveAnimation : IRouterAnimation<FromTo>
    {
        private readonly CanvasGroup canvasGroup;
        
        private Tween fadeTween;
        
        public ActItem<FromTo> Animate { get; }
        
        public MainMenuLeaveAnimation(UIAnimationsService uiAnimationsService)
        {
            var uiMap = uiAnimationsService.GetResourceMap<MainMenuScreenResourceMap>();
            canvasGroup = uiMap.CanvasGroup;
            
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