using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Suburb.Utils
{
    [Serializable]
    public class Vector3Data
    {
        [JsonProperty("x")]
        public float X;

        [JsonProperty("y")]
        public float Y;

        [JsonProperty("z")]
        public float Z;

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
