using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Suburb.Common
{
    [Serializable]
    public class GameResourcesData
    {
        [JsonProperty("rovers_data")]
        public AssetsCategory<RoverData> RoversData;
    }
}
