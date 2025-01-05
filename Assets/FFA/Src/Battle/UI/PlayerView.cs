using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.Battle.UI
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] PlayerHealthIndicator healthIndicator;
        [SerializeField] private Graphic playerBody;
        [SerializeField] private Graphic playerBodyBorder;
        [SerializeField] private Graphic background;
        
        public class Pool : MonoMemoryPool<PlayerView>
        {
        
        }
    }
}