using System;
using Newtonsoft.Json;
using Suburb.Utils.Serialization;

namespace Suburb.Serialization
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
