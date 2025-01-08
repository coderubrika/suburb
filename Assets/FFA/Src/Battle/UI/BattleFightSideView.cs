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
        [SerializeField] private float outOfZoneAlpha;
        
        [Inject]
        private void Construct(BattleService battleService)
        {
            this.battleService = battleService;
        }
        
        public void Init()
        {
            graphic.color = UIUtils.GetNewAlpha(battleService.GetColor(side), 0);
        }

        public void Show()
        {
            PlaySide(baseAlpha);
        }
        
        public void Hide()
        {
            DOTween.Kill(graphic);
            graphic.color = UIUtils.GetNewAlpha(Color.white, 0);
        }
        
        private void PlaySide(float alpha)
        {
            graphic.DOFade(alpha, 0.4f);
        }
    }
}