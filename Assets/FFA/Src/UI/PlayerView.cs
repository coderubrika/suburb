using UnityEngine;
using UnityEngine.UI;

namespace FFA.UI
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] PlayerHealthIndicator healthIndicator;
        [SerializeField] private Graphic playerBody;
        [SerializeField] private Graphic playerBodyBorder;
        [SerializeField] private Graphic background;
    }
}