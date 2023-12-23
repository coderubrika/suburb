using System;
using DG.Tweening;
using Suburb.Common;
using Suburb.ExpressRouter;
using Suburb.ResourceMaps;
using Suburb.Utils;
using Suburb.Utils.Serialization;
using UnityEngine;

namespace Suburb.UI
{
    public class FadeCanvasGroupAnimation
    {
        private readonly CanvasGroup canvasGroup;
        private readonly ValueAnimationData<float> config;
        
        private Tween fadeTween;
        
        public ActItem<FromTo> Animate { get; }
        
        public FadeCanvasGroupAnimation(CanvasGroup canvasGroup, ValueAnimationData<float> config)
        {
            this.canvasGroup = canvasGroup;
            this.config = config;
            
            Animate = new ActItem<FromTo>(Invoke, Finally);
        }

        private void Invoke(FromTo points, Action<FromTo> next)
        {
            fadeTween = UIUtils.FadeCanvas(canvasGroup, config)
                .OnComplete(() => next.Invoke(points));
        }

        private void Finally()
        {
            fadeTween?.Kill();
        }
    }
}