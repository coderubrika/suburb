using System;
using UnityEngine;

namespace Suburb.Common
{
    [CreateAssetMenu(fileName = "GameSettingsRepository", menuName = "Repositories/GameSettingsRepository")]
    public class GameSettingsRepository : ScriptableObject
    {
        [SerializeField] private WorldCameraControllerSettings worldCameraControllerSettings;
        [SerializeField] private GameCollectedData defaultSaveData;

        public WorldCameraControllerSettings WorldCameraControllerSettings => worldCameraControllerSettings;
        public GameCollectedData DefaultSaveData => defaultSaveData;
    }
}
