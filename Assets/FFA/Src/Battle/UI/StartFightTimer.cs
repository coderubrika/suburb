using System;
using DG.Tweening;
using Suburb.Utils;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace FFA.Battle.UI
{
    public class StartFightTimer : MonoBehaviour
    {
        [SerializeField] private Image[] images;

        private bool isReset;
        private Sequence ending;
        
        public void ResetTimer()
        {
            isReset = true;
            ending?.Kill();
            
            foreach (var image in images)
                image.fillAmount = 0;
        }

        public IObservable<Unit> StartTimer(TimeSpan time)
        {
            isReset = false;
            foreach (var image in images)
                image.fillAmount = 1;
            
            float timer = 0.0f;
            return Observable.EveryUpdate()
                .ObserveOnMainThread()
                .Do(_ =>
                {
                    timer += Time.deltaTime;
                    foreach (var image in images)
                        image.fillAmount = Mathf.Clamp01(1 - timer / (float)time.TotalSeconds);
                })
                .Where(_ => timer >= time.TotalSeconds || isReset)
                .Take(1)
                .Select(_ => Unit.Default);
        }
        
        public IObservable<Unit> GoneTimer()
        {
            isReset = true;
            ending?.Kill();
            ending = DOTween.Sequence();
            foreach (var image in images)
                ending.Join(image.DOFillAmount(0, UIUtils.GetDurationForPercentage0(image.fillAmount, 0.4f)).SetEase(Ease.InSine));
            return ending.ToObservableOnKill();
        }
    }
}