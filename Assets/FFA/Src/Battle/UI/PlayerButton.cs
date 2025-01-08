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

        private BattleSide side;

        public Button Button => button;
        
        private void Setup(BattleSide side, PlayerData data)
        {
            this.side = side;
            playerBody.color = data.BodyColor;
            playerBodyBorder.color = data.BodyBorderColor;
            background.color = data.BackgroundColor;
        }
        
        public class Pool : MonoMemoryPool<BattleSide, PlayerData, PlayerButton>
        {
            protected override void Reinitialize(BattleSide side, PlayerData data, PlayerButton item)
            {
                item.Setup(side, data);
            }
        }
    }
}