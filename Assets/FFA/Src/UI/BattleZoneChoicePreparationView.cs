using System;
using DG.Tweening;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FFA.UI
{
    public class BattleZoneChoicePreparationView : MonoBehaviour
    {
        [SerializeField] private AnnouncementOfStartBattle announcementOfStartBattle;
        [SerializeField] private Image spawnPlayersImage;
        
        private float spawnPlayersAlpha;
        
        private void Awake()
        {
            spawnPlayersAlpha = spawnPlayersImage.color.a;
        }

        public void Init()
        {
            spawnPlayersImage.gameObject.SetActive(true);
            spawnPlayersImage.color = UIUtils.GetNewAlpha(spawnPlayersImage.color, spawnPlayersAlpha);
            announcementOfStartBattle.gameObject.SetActive(false);
        }
        
        public IObservable<Unit> Show(string opponentName)
        {
            return spawnPlayersImage.DOFade(0, 0.4f).ToObservableOnKill()
                .ObserveOnMainThread()
                .ContinueWith(_ =>
                {
                    spawnPlayersImage.gameObject.SetActive(false);
                    announcementOfStartBattle.gameObject.SetActive(true);
                    return announcementOfStartBattle.PlayStartBattle(opponentName);
                })
                .Do(_ => announcementOfStartBattle.gameObject.SetActive(false));
        }

        public void Hide()
        {
            announcementOfStartBattle.Hide();
            announcementOfStartBattle.gameObject.SetActive(false);
        }
    }
}