using Newtonsoft.Json;
using Suburb.Utils;
using System;

namespace Suburb.Common
{
    [Serializable]
    public class WorldCameraControllerData : IEquatable<WorldCameraControllerData>
    {
        [JsonProperty("position")]
        public Vector3Data Position;

        [JsonProperty("zoom")]
        public float Zoom;

        public bool Equals(WorldCameraControllerData other)
        {
            return Position.Equals(other.Position) && Zoom.Equals(other.Zoom);
        }
    }
}
