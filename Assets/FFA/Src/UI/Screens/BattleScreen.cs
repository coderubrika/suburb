using System;
using DG.Tweening;
using Suburb.Screens;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.UI.Screens
{
    public class BattleScreen : BaseScreen
    {
        [SerializeField] private StartFightTimer startFightTimer;
        [SerializeField] private Image fieldFader;
        [SerializeField] private BattleZoneChoicePreparationView zoneDown;
        [SerializeField] private BattleZoneChoicePreparationView zoneTop;
        
        private readonly CompositeDisposable disposables = new();

        private float fieldFaderAlpha;
        
        [Inject]
        private void Construct()
        {
            fieldFaderAlpha = fieldFader.color.a;
        }
        
        protected override void Show()
        {
            base.Show();
            fieldFader.gameObject.SetActive(true);
            fieldFader.color = UIUtils.GetNewAlpha(fieldFader.color, fieldFaderAlpha);
            zoneDown.gameObject.SetActive(true);
            zoneDown.Init(2);
            
            zoneTop.gameObject.SetActive(true);
            zoneTop.Init(2);
            
            startFightTimer.gameObject.SetActive(true);
            startFightTimer.ResetTimer();
            
            Observable.WhenAll(zoneDown.OnResponse.Take(1), zoneTop.OnResponse.Take(1))
                .ObserveOnMainThread()
                .ContinueWith(_ => startFightTimer.StartTimer(TimeSpan.FromSeconds(0.5f)))
                .ObserveOnMainThread()
                .Do(_ => startFightTimer.gameObject.SetActive(false))
                .ObserveOnMainThread()
                .ContinueWith(_ => Observable.WhenAll(
                    zoneDown.Show("Player2"), 
                    zoneTop.Show("Player1")))
                .ContinueWith(_ => fieldFader.DOFade(0, 0.4f).ToObservableOnComplete())
                .Subscribe()
                .AddTo(disposables);
        }

        protected override void Hide()
        {
            disposables.Clear();
            zoneDown.Hide();
            zoneTop.Hide();
            startFightTimer.ResetTimer();
            DOTween.Kill(fieldFader);
            startFightTimer.gameObject.SetActive(false);
        }
    }
}