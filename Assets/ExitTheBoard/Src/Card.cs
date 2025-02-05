using UnityEngine;

namespace ExitTheBoard
{
    public class Card : MonoBehaviour, IMovable
    {
        [SerializeField] private BoxCollider boxCollider;
        public Transform Transform => transform;
        public BoxCollider BoxCollider => boxCollider;
    }
}