using Newtonsoft.Json;
using Suburb.Utils;
using System;

namespace Suburb.Serialization
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

        [JsonProperty("world_camera_controller_data")]
        public WorldCameraControllerData WorldCameraControllerData;

        [JsonProperty("game_resources_data")]
        public GameResourcesData GameResourcesData;

        [JsonIgnore]
        public bool IsDataHasChanges { get; private set; }

        public void Rename(string name)
        {
            IsDataHasChanges = true;
            Name = name;
        }

        public void UpdateSaveTime()
        {
            IsDataHasChanges = false;
            SaveTime = DateTimeUtils.GetDetailNow();
        }

        public void Replace(GameCollectedData donar)
        {
            IsDataHasChanges = true;
            Name = donar.Name;
            WorldCameraControllerData = donar.WorldCameraControllerData;
            GameResourcesData = donar.GameResourcesData;
        }

        public void UpdateWorldCameraControllerData(WorldCameraControllerData data)
        {
            if (data.Equals(WorldCameraControllerData))
                return;

            IsDataHasChanges = true;
            WorldCameraControllerData = data;
        }
    }
}
