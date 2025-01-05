using System;
using System.Linq;
using DG.Tweening;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.Battle.UI
{
    public class BattlePreparationView : MonoBehaviour
    {
        private BattleService battleService;
            
        [SerializeField] private StartFightTimer startFightTimer;
        [SerializeField] private Image fieldFader;
        [SerializeField] private BattleZoneChoicePreparationView[] zones;

        private readonly CompositeDisposable disposables = new();

        private float fieldFaderAlpha;
        
        [Inject]
        private void Construct(BattleService battleService)
        {
            this.battleService = battleService;
            fieldFaderAlpha = fieldFader.color.a;
        }
        
        public IObservable<Unit> Show()
        {
            fieldFader.gameObject.SetActive(true);
            fieldFader.color = UIUtils.GetNewAlpha(fieldFader.color, fieldFaderAlpha);
            foreach (var zone in zones)
            {
                zone.gameObject.SetActive(true);
                zone.Init(battleService.GetPlayersCount(zone.BattleSide));
                zone.OnResponse
                    .Subscribe(position => battleService.SetupPlayer(zone.BattleSide, position))
                    .AddTo(disposables);
            }
            
            startFightTimer.gameObject.SetActive(true);
            startFightTimer.ResetTimer();
            
            return zones.Select(zone => zone.OnResponse.Take(1)).WhenAll()
                .ObserveOnMainThread()
                .ContinueWith(_ => startFightTimer.StartTimer(TimeSpan.FromSeconds(1)))
                .ObserveOnMainThread()
                .Do(_ => startFightTimer.gameObject.SetActive(false))
                .ObserveOnMainThread()
                .ContinueWith(_ => zones.Select(zone => zone.Show(battleService.GetName(zone.BattleSide))).WhenAll()
                .ObserveOnMainThread()
                .ContinueWith(_ => fieldFader.DOFade(0, 0.4f).ToObservableOnComplete()));
        }

        public void Hide()
        {
            disposables.Clear();
            foreach (var zone in zones)
            {
                zone.Hide();
                zone.gameObject.SetActive(false);
            }
            startFightTimer.ResetTimer();
            startFightTimer.gameObject.SetActive(false);
            DOTween.Kill(fieldFader);
            fieldFader.gameObject.SetActive(false);
        }
    }
}