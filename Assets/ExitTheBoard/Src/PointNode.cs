using UnityEngine;

namespace ExitTheBoard
{
    public class PointNode
    {
        public Vector3 Position { get; }
        public PointNode[] NeighboursPoints { get; private set; }

        public PointNode(Vector3 position)
        {
            Position = position;
        }

        public void SetNeighboursPoints(PointNode[] neighbours)
        {
            NeighboursPoints = neighbours;
        }
        
        
    }
}