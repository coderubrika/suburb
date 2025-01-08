using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.Battle.UI
{
    public class PlayerButton : MonoBehaviour
    {
        [SerializeField] private Graphic playerBody;
        [SerializeField] private Graphic playerBodyBorder;
        [SerializeField] private Graphic background;
        [SerializeField] private Button button;

        public class Pool : MonoMemoryPool<BattleSide, PlayerData, PlayerButton>
        {
            protected override void Reinitialize(BattleSide side, PlayerData data, PlayerButton item)
            {
                
            }
        }
    }
}