using UnityEngine;

namespace FFA.Battle.UI
{
    public class BattleFinalSide : MonoBehaviour
    {
        [SerializeField] private BattleSide side;
        [SerializeField] private GameObject win;
        [SerializeField] private GameObject defeat;

        public void SetWinSide(BattleSide winSide)
        {
            win.SetActive(winSide == side);
            defeat.SetActive(winSide != side);
        }
    }
}