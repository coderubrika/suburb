using System;
using Newtonsoft.Json;

namespace Suburb.Serialization
{
    [Serializable]
    public class GameResourcesData
    {
        [JsonProperty("rovers_data")]
        public AssetsCategory<RoverData> RoversData;
    }
}
