using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Suburb.ExpressRouter;
using Suburb.Utils.Serialization;
using UnityEngine;

namespace Suburb.UI
{
    public class CompositeAnimation
    {
        private readonly CanvasGroup canvasGroup;
        private readonly ValueStartEndAnimationData<float> config;
        private readonly List<Func<FromTo, Tween>> tweenGetters = new();
        
        private Sequence sequence;
        
        public ActItem<FromTo> Animate { get; }
        
        public CompositeAnimation()
        {
            Animate = new ActItem<FromTo>(Invoke, Finally);
        }
        
        public CompositeAnimation(params Func<FromTo, Tween>[] tweenGetters)
        {
            foreach (var getter in tweenGetters)
                this.tweenGetters.Add(getter);
            Animate = new ActItem<FromTo>(Invoke, Finally);
        }

        public void AddTweenGetter(Func<FromTo, Tween> tweenGetter)
        {
            tweenGetters.Add(tweenGetter);
        }
        
        private void Invoke(FromTo points, Action<FromTo> next)
        {
            sequence = DOTween.Sequence().OnComplete(() => next.Invoke(points));
            foreach (var getter in tweenGetters)
                sequence.Append(getter.Invoke(points));
        }

        private void Finally()
        {
            sequence?.Kill();
        }
    }
}