using System;
using Newtonsoft.Json;

namespace Suburb.Common
{
    [Serializable]
    public class SavesData
    {
        [JsonProperty("file_names")]
        public string[] FileNames;

        [JsonProperty("last_save_uid")]
        public string LastSaveUID;
    }
}
