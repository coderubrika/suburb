using UnityEngine;

namespace ExitTheBoard
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;
        public Transform Transform => transform;
        public BoxCollider BoxCollider => boxCollider;
    }
}