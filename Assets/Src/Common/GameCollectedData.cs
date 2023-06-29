using Newtonsoft.Json;
using Suburb.Utils;
using System;
using System.Collections.Generic;

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

        [JsonProperty("world_camera_controller_data")]
        public WorldCameraControllerData WorldCameraControllerData;

        [JsonProperty("game_resources_data")]
        public GameResourcesData GameResourcesData;

        public void Rename(string name)
        {
            Name = name;
        }

        public void UpdateSaveTime()
        {
            SaveTime = DateTimeUtils.GetDetailNow();
        }

        public void Replace(GameCollectedData donar)
        {
            Name = donar.Name;
            WorldCameraControllerData = donar.WorldCameraControllerData;
            GameResourcesData = donar.GameResourcesData;
        }

        public void UpdateWorldCameraControllerData(WorldCameraControllerData data)
        {
            WorldCameraControllerData = data;
        }
    }
}
