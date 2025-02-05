using UnityEngine;

namespace ExitTheBoard
{
    public interface IMovable
    {
        public Transform Transform { get; }
        public BoxCollider BoxCollider { get; }
    }
}