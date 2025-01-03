using System;
using DG.Tweening;
using Suburb.Utils;
using TMPro;
using UniRx;
using UnityEngine;

namespace FFA.UI
{
    public class AnnouncementOfStartBattle : MonoBehaviour
    {
        [SerializeField] private TMP_Text opponentNameText;
        [SerializeField] private RectTransform nameRect;
        [SerializeField] private RectTransform fightRect;
        [SerializeField] private Vector2 startNameAnchoredPosition;
        [SerializeField] private Vector2 viewNameAnchoredPosition;
        [SerializeField] private Vector2 endNameAnchoredPosition;
        [SerializeField] private Vector2 startFightAnchoredPosition;
        [SerializeField] private Vector2 viewFightAnchoredPosition;
        [SerializeField] private Vector2 endFightAnchoredPosition;

        public IObservable<Unit> PlayStartBattle(string opponentName)
        {
            opponentNameText.text = opponentName;
            nameRect.anchoredPosition = startNameAnchoredPosition;
            fightRect.anchoredPosition = startFightAnchoredPosition;
            
            fightRect.gameObject.SetActive(true);
            nameRect.gameObject.SetActive(true);
            
            DOTween.Kill(fightRect);
            DOTween.Kill(nameRect);
            
            return DOTween.Sequence()
                .Append(nameRect.DOAnchorPos(viewNameAnchoredPosition, 0.9f).SetEase(Ease.InOutCubic))
                .AppendInterval(0.2f)
                .Append(nameRect.DOAnchorPos(endNameAnchoredPosition, 0.4f).SetEase(Ease.InOutSine))
                .Append(fightRect.DOAnchorPos(viewFightAnchoredPosition, 0.9f).SetEase(Ease.InOutCubic))
                .AppendInterval(0.2f)
                .Append(fightRect.DOAnchorPos(endFightAnchoredPosition, 0.6f).SetEase(Ease.InOutSine))
                .ToObservableOnKill();
        }

        public void Hide()
        {
            DOTween.Kill(fightRect);
            DOTween.Kill(nameRect);
            fightRect.gameObject.SetActive(false);
            nameRect.gameObject.SetActive(false);
        }
    }
}