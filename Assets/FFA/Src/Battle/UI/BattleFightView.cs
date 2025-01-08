using UnityEngine;

namespace FFA.Battle.UI
{
    public class BattleFightView : MonoBehaviour
    {
        [SerializeField] private BattleFightSideView[] sideViews;
        
        public void Init()
        {
            foreach (var view in sideViews)
                view.Init();
        }
        
        public void Show()
        {
            foreach (var view in sideViews)
                view.Show();
        }

        public void Hide()
        {
            foreach (var view in sideViews)
                view.Hide();
        }
    }
}