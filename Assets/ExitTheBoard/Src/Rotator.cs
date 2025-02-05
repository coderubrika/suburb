using UnityEngine;

namespace ExitTheBoard
{
    public class Rotator : MonoBehaviour, IMovable, IClickable
    {
        [SerializeField] private BoxCollider boxCollider;
        public Transform Transform => transform;
        public BoxCollider BoxCollider => boxCollider;
    }
}