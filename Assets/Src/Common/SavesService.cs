using Suburb.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Suburb.Common
{
    public class SavesService
    {
        private readonly LocalStorageService localStorageService;
        private readonly GameSettingsRepository gameSettingsRepository;

        private readonly Dictionary<string, GameCollectedData> saves;

        private static string SAVES_FOLDER = "saves";
        private static string SAVES_DATA_PATH = Path.Combine(SAVES_FOLDER, "savesData.json");

        public GameCollectedData SelectedData { get; private set; }

        public SavesService(
            LocalStorageService localStorageService,
            GameSettingsRepository gameSettingsRepository)
        {
            this.localStorageService = localStorageService;
            this.gameSettingsRepository = gameSettingsRepository;

            localStorageService.CreatePersistentFolder(SAVES_FOLDER);

            var savesData = localStorageService.LoadFromPersistent<SavesData>(SAVES_DATA_PATH);
            
            if (savesData == null || savesData.FileNames == null)
            {
                saves = new();
                return;
            }

            saves = savesData.FileNames
                .Select(fileName =>
                {
                    var data = localStorageService
                        .LoadFromPersistent<GameCollectedData>(Path.Combine(SAVES_FOLDER, fileName));
                    return data;
                })
                .ToDictionary(item => item.UID, item => item);

            if (string.IsNullOrEmpty(savesData.LastSaveUID))
                return;

            if (saves.TryGetValue(savesData.LastSaveUID, out GameCollectedData data))
                SelectedData = data;
        }

        public void Create()
        {
            string uid = GeneralUtils.GetUID();
            var gameData = new GameCollectedData()
            {
                UID = uid,
                SaveTime = DateTimeUtils.GetDetailNow(),
                FileName = "save_" + uid + ".json"
            };

            gameData.Replace(gameSettingsRepository.DefaultSaveData);

            SelectedData = gameData;
        }

        public void Save()
        {
            if (SelectedData == null)
                return;

            SelectedData.UpdateSaveTime();
            saves[SelectedData.UID] = SelectedData;
            localStorageService.SaveToPersistent(Path.Combine(SAVES_FOLDER, SelectedData.FileName), SelectedData);
            SyncData();
        }

        public void SaveAsNew()
        {
            if (SelectedData == null)
                return;

            GameCollectedData oldSelectedData = SelectedData;
            Create();
            SelectedData.Replace(oldSelectedData);
            Save();
        }

        public void SaveAs(string uid)
        {
            if (SelectedData == null)
                return;

            if (!saves.TryGetValue(uid, out GameCollectedData recipientData))
                return;

            recipientData.Replace(SelectedData);
            recipientData.UpdateSaveTime();
            SelectedData = recipientData;
            localStorageService.SaveToPersistent(Path.Combine(SAVES_FOLDER, recipientData.FileName), recipientData);
        }

        public void Delete(string uid)
        {
            if (!saves.TryGetValue(uid, out GameCollectedData deletingData))
                return;

            saves.Remove(uid);
            localStorageService.RemoveFile(Path.Combine(SAVES_FOLDER, deletingData.FileName));
            SyncData();
        }

        public void SyncData()
        {
            string[] fileNames = saves.Values
                .Select(item => item.FileName)
                .ToArray();

            localStorageService.SaveToPersistent(
                SAVES_DATA_PATH,
                new SavesData { FileNames = fileNames, LastSaveUID = SelectedData?.UID });
        }

        public void Rename(string uid, string name)
        {
            saves[uid].Rename(name);
        }

        public GameCollectedData[] GetSaves()
        {
            return saves.Values.ToArray();
        }

        public void Select(GameCollectedData data)
        {
            SelectedData = data;
        }
    }
}
