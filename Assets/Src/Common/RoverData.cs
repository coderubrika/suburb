using System;
using Newtonsoft.Json;
using Suburb.Utils;

namespace Suburb.Common
{
    [Serializable]
    public class RoverData
    {
        [JsonProperty("position")]
        public Vector3Data Position;

        [JsonProperty("rotation")]
        public Vector3Data Rotation;
    }
}
