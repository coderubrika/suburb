using System;
using UnityEngine;

namespace ExitTheBoard
{
    [Serializable]
    public class LineTrack
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;

        public Vector3 DirectionOne => (EndPoint - StartPoint).normalized;
    }
}