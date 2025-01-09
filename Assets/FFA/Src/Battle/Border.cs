using FFA.Battle.UI;
using Suburb.Utils;
using UnityEngine;

namespace FFA.Battle
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Border : MonoBehaviour
    {
        private enum Orientation
        {
            Horizontal,
            Vertical
        }

        [SerializeField] private Orientation orientation;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerView playerView = other.gameObject.GetComponent<PlayerView>();
            if (playerView == null)
                return;
            
            Vector2 velocity = playerView.Rigidbody.velocity;
            velocity = orientation == Orientation.Vertical
                ? velocity.ChangeX(-velocity.x)
                : velocity.ChangeY(-velocity.y);
            playerView.Rigidbody.velocity = velocity;
        }
    }
}