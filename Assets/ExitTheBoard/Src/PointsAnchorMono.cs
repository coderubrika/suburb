using System;
using UnityEngine;

namespace ExitTheBoard
{
    public class PointsAnchorMono : MonoBehaviour
    {
        [SerializeField] private PointNodeMono startPoint;
        [SerializeField] private PointNodeMono endPoint;

        public (PointNode StartPoint, int EndPointIdx) GetStartEndPoints()
        {
            PointNode pointNode = startPoint.Scan();
            int idx = Array.IndexOf(pointNode.NeighboursPoints, endPoint.Scan());
            return (pointNode, idx);
        }
    }
}