using Newtonsoft.Json;
using System;

namespace Suburb.Common
{
    [Serializable]
    public class GameCollectedData
    {
        [JsonProperty]
        public string SaveTime;
    }
}
