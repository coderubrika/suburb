using System;
using UnityEngine;

namespace ExitTheBoard
{
    public class LineTrack
    {
        public Vector3 StartPoint { get; }
        public Vector3 EndPoint { get; }

        public Vector3 Direction => EndPoint - StartPoint;
        public Vector3 DirectionOne => Direction.normalized;
        public float Length => Direction.magnitude;
        
        public LineTrack(Vector3 startPoint, Vector3 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }
}