using Suburb.Utils;
using System;
using UnityEngine;

namespace Suburb.Common
{
    [Serializable]
    public class WorldCameraControllerSettings
    {
        [SerializeField] private SmoothTransitionParam smoothTransitionParam;
        [SerializeField] private float zoomFactor;
        [SerializeField] private float maxZoom;
        [SerializeField] private float minZoom;
        [SerializeField] private float maxMoveSensivityFactor;
        [SerializeField] private float minMoveSensivity;

        public SmoothTransitionParam SmoothTransitionParam => smoothTransitionParam;
        public float ZoomFactor => zoomFactor;
        public float MaxZoom => maxZoom;
        public float MinZoom => minZoom;
        public float MaxMoveSensivityFactor => maxMoveSensivityFactor;
        public float MinMoveSensivity => minMoveSensivity;
    }
}
