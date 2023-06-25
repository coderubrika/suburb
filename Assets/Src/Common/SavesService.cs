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

        private readonly Dictionary<string, GameCollectedData> saves;
        private readonly HashSet<string> changedDatasUIDs = new();
        private readonly HashSet<string> savesFileNames;

        private static string SAVES_FOLDER = "saves";
        private static string SAVES_LIST_FILE_PATH = Path.Combine(SAVES_FOLDER, "savesList.json");

        private GameCollectedData selectedData;

        public SavesService(LocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;

            var savesNamesArray = localStorageService
                .LoadFromPersistent<string[]>(SAVES_LIST_FILE_PATH);
            savesFileNames = savesNamesArray == null ? new() : savesNamesArray.ToHashSet();

            saves = savesFileNames
                .Select(fileName =>
                {
                    var data = localStorageService
                        .LoadFromPersistent<GameCollectedData>(Path.Combine(SAVES_FOLDER, fileName));
                    return data;
                })
                .ToDictionary(item => item.UID, item => item);

            Select(GetLast().UID);
        }

        public int SavesCount { get => saves.Count; }

        public GameCollectedData Create()
        {
            string uid = GeneralUtils.GetUID();
            var gameData = new GameCollectedData()
            {
                UID = uid,
                SaveTime = DateTimeUtils.GetNow(),
                Name = "Новое сохранение",
                FileName = "save_" + uid + ".json"
            };

            saves.Add(gameData.UID, gameData);
            changedDatasUIDs.Add(gameData.UID);
            savesFileNames.Add(gameData.FileName);

            return gameData;
        }

        public GameCollectedData GetSelectedData()
        {
            return selectedData;
        }

        public void Select(string configUID)
        {
            selectedData = saves[configUID];
        }

        public void Update(string uid)
        {
            saves[uid].UpdateSaveTime();
        }

        public void Save()
        {
            foreach (var uid in changedDatasUIDs)
            {
                GameCollectedData data = saves[uid];
                localStorageService.SaveToPersistent(Path.Combine(SAVES_FOLDER, data.FileName), data);
            }

            localStorageService.SaveToPersistent(SAVES_LIST_FILE_PATH, savesFileNames);
        }

        public void Delete(string uid)
        {
            saves.Remove(uid);
            changedDatasUIDs.Remove(uid);
            savesFileNames.Remove(uid);

            localStorageService.RemoveFile(SAVES_LIST_FILE_PATH);
            localStorageService.SaveToPersistent(SAVES_LIST_FILE_PATH, savesFileNames);
        }

        public void Rename(string uid, string name)
        {
            saves[uid].Rename(name);
        }

        private GameCollectedData GetLast()
        {
            if (SavesCount == 0)
                return null;

            return saves.Values
                .OrderBy(data => data.UID)
                .LastOrDefault();
        }
    }
}
