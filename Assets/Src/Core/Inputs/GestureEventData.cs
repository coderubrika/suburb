using UnityEngine;

namespace Suburb.Core.Inputs
{
    public class GestureEventData
    {
        public int Id;
        public Vector2 Position;
        public Vector2 Delta;
        public Vector2 ZoomDelta;
        public GestureType Type;
    }
}
