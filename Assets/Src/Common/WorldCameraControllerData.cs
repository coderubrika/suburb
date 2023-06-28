using Newtonsoft.Json;
using Suburb.Utils;
using System;

namespace Suburb.Common
{
    [Serializable]
    public class WorldCameraControllerData
    {
        [JsonProperty("position")]
        public Vector3Data Position;

        [JsonProperty("zoom")]
        public float Zoom;
    }
}
