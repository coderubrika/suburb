using System;
using DG.Tweening;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.Battle.UI
{
    public class BattleFinalView : MonoBehaviour
    {
        [SerializeField] private Button play;
        [SerializeField] private Button back;
        [SerializeField] private BattleFinalSide topSide;
        [SerializeField] private BattleFinalSide bottomSide;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Graphic fader;
        [SerializeField] private float faderAlpha;
        
        public Button Play => play;
        public Button Back => back;
        
        
        public void Init()
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
        }

        public void Show(BattleSide side)
        {
            fader.color = UIUtils.GetNewAlpha(fader.color, 0);
            fader.gameObject.SetActive(true);
            topSide.SetWinSide(side);
            bottomSide.SetWinSide(side);

            fader.DOFade(faderAlpha, 0.3f);
            canvasGroup.DOFade(1, 0.4f)
                .OnComplete(() => canvasGroup.interactable = true);
        }

        public IObservable<Unit> PlayHide()
        {
            DOTween.Kill(canvasGroup);
            canvasGroup.interactable = false;
            return DOTween.Sequence()
                .Join(fader.DOFade(0, 0.4f))
                .Join(canvasGroup.DOFade(0, 0.4f))
                .ToObservableOnComplete();
        }

        public void Hide()
        {
            DOTween.Kill(canvasGroup);
            DOTween.Kill(fader);
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
            fader.color = UIUtils.GetNewAlpha(fader.color, 0);
            fader.gameObject.SetActive(false);
        }
    }
}