using DG.Tweening;
using Suburb.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.Battle.UI
{
    public class BattleFightSideView : MonoBehaviour
    {
        private BattleService battleService;
        
        [SerializeField] private BattleSide side;
        [SerializeField] private Graphic graphic;
        [SerializeField] private float baseAlpha;
        [SerializeField] private float activeAlpha;
        [SerializeField] private RectTransform rectTransform;
        
        public RectTransform RectTransform => rectTransform;
        public BattleSide Side => side;
        
        private float targetAlpha;
        
        [Inject]
        private void Construct(BattleService battleService)
        {
            this.battleService = battleService;
        }
        
        public void Init()
        {
            targetAlpha = 0;
            graphic.color = UIUtils.GetNewAlpha(battleService.GetColor(side), 0);
        }

        public void PlayBase()
        {
            PlaySide(baseAlpha);
        }

        public void PlayActive()
        {
            PlaySide(activeAlpha);
        }
        
        public void Clear()
        {
            targetAlpha = 0;
            DOTween.Kill(graphic);
            graphic.color = UIUtils.GetNewAlpha(Color.white, 0);
        }
        
        private void PlaySide(float alpha)
        {
            if (targetAlpha == alpha)
                return;
            targetAlpha = alpha;
            
            DOTween.Kill(graphic);
            graphic.DOFade(alpha, 0.4f);
        }
    }
}