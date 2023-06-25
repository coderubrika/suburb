using Newtonsoft.Json;
using Suburb.Utils;
using System;

namespace Suburb.Common
{
    [Serializable]
    public class GameCollectedData
    {
        [JsonProperty("uid")]
        public string UID;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("save_time")]
        public string SaveTime;

        [JsonProperty("file_name")]
        public string FileName;

        public void Rename(string name)
        {
            Name = name;
        }

        public void UpdateSaveTime()
        {
            SaveTime = DateTimeUtils.GetNow();
        }
    }
}
