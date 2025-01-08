using UnityEngine;

namespace FFA.Battle.UI
{
    public class BattleFightView : MonoBehaviour
    {
        [SerializeField] private BattleFightSideView[] sideViews;
        private  readonly BattleFightSideView[] sortedSideViews = new BattleFightSideView[2];

        private void Awake()
        {
            foreach (var sideView in sideViews)
                sortedSideViews[(int)sideView.Side] = sideView;
        }

        public BattleFightSideView GetSideView(BattleSide side) => sortedSideViews[(int)side];
        
        public void Init()
        {
            foreach (var view in sideViews)
                view.Init();
        }
        
        public void Show()
        {
            foreach (var view in sideViews)
                view.PlayBase();
        }

        public void Hide()
        {
            foreach (var view in sideViews)
                view.Clear();
        }
    }
}