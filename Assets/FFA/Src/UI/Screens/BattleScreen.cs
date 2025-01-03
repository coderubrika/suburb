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
        [SerializeField] private BattleZoneChoicePreparationView zoneChoicePreparationViewDown;
        [SerializeField] private BattleZoneChoicePreparationView zoneChoicePreparationViewTop;
        
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
            zoneChoicePreparationViewDown.gameObject.SetActive(true);
            zoneChoicePreparationViewDown.Init();
            
            zoneChoicePreparationViewTop.gameObject.SetActive(true);
            zoneChoicePreparationViewTop.Init();
            
            startFightTimer.gameObject.SetActive(true);
            startFightTimer.ResetTimer();
            startFightTimer.StartTimer(TimeSpan.FromSeconds(0.5f))
                .ObserveOnMainThread()
                .Do(_ => startFightTimer.gameObject.SetActive(false))
                .ObserveOnMainThread()
                .ContinueWith(_ => Observable.WhenAll(
                    zoneChoicePreparationViewDown.Show("Player2"), 
                    zoneChoicePreparationViewTop.Show("Player1")))
                .ContinueWith(_ => fieldFader.DOFade(0, 0.4f).ToObservableOnKill())
                .Subscribe()
                .AddTo(disposables);
        }

        protected override void Hide()
        {
            disposables.Clear();
            startFightTimer.ResetTimer();
            startFightTimer.gameObject.SetActive(false);
        }
    }
}