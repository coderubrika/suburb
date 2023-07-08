using Suburb.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;

namespace Suburb.Common
{
    public class SavesService
    {
        private readonly LocalStorageService localStorageService;
        private readonly GameSettingsRepository gameSettingsRepository;

        private readonly Dictionary<string, GameCollectedData> saves;

        private const string SAVES_FOLDER = "saves";
        private const string SAVES_DATA_PATH = "saves/savesData.json";

        private GameCollectedData selectedData;

        public GameCollectedData TmpData { get; } = new GameCollectedData();
        public bool HasSelectedSave { get => selectedData != null; }
        public bool IsSelectedSaved { get => HasSelectedSave && saves.ContainsKey(selectedData.UID); }
        public bool HasChanges { get => TmpData.IsDataHasChanges; }
        public Subject<ChangeType> OnChangeSaves { get; } = new();

        public enum ChangeType { Rewrite, Save, Delete }

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
                 Select(data);
        }

        public void SaveAsNew()
        {
            if (!HasSelectedSave)
                return;

            GameCollectedData newSelectedData = Create();
            newSelectedData.Replace(TmpData);
            TmpData.UpdateSaveTime();
            Save(newSelectedData);
        }

        public void SaveAs(string uid)
        {
            if (!HasSelectedSave)
                return;

            if (!saves.TryGetValue(uid, out GameCollectedData recipientData))
                return;

            recipientData.Replace(TmpData);
            recipientData.UpdateSaveTime();
            TmpData.UpdateSaveTime();

            localStorageService.SaveToPersistent(Path.Combine(SAVES_FOLDER, recipientData.FileName), recipientData);
            OnChangeSaves.OnNext(ChangeType.Rewrite);
        }

        public void Delete(string uid)
        {
            if (!saves.TryGetValue(uid, out GameCollectedData deletingData))
                return;

            if (selectedData != null && selectedData.UID == uid)
                selectedData = saves.Select(item => item.Value).LastOrDefault();

            saves.Remove(uid);
            localStorageService.RemoveFile(Path.Combine(SAVES_FOLDER, deletingData.FileName));
            SyncData();
            OnChangeSaves.OnNext(ChangeType.Delete);
        }

        public void Rename(string uid, string name)
        {
            saves[uid].Rename(name);
        }

        public GameCollectedData[] GetSaves()
        {
            return saves.Values.Reverse().ToArray();
        }

        public void CreateNewSave()
        {
            Select(Create());
        }

        public void Select(GameCollectedData data)
        {
            selectedData = data;
            TmpData.Replace(selectedData);
            TmpData.UpdateSaveTime();
        }

        public void SaveSelected()
        {
            if (!HasChanges || !IsSelectedSaved)
                return;

            SaveAs(selectedData.UID);
        }

        private GameCollectedData Create()
        {
            string uid = GeneralUtils.GetUID();
            var gameData = new GameCollectedData()
            {
                UID = uid,
                SaveTime = DateTimeUtils.GetDetailNow(),
                FileName = "save_" + uid + ".json"
            };

            gameData.Replace(gameSettingsRepository.DefaultSaveData);
            return gameData;
        }

        private void Save(GameCollectedData data)
        {
            if (!HasSelectedSave)
                return;

            data.UpdateSaveTime();
            saves[data.UID] = data;
            localStorageService.SaveToPersistent(Path.Combine(SAVES_FOLDER, data.FileName), data);
            SyncData();
            OnChangeSaves.OnNext(ChangeType.Save);
        }

        private void SyncData()
        {
            string[] fileNames = saves.Values
                .Select(item => item.FileName)
                .ToArray();

            localStorageService.SaveToPersistent(
                SAVES_DATA_PATH,
                new SavesData { FileNames = fileNames, LastSaveUID = selectedData?.UID });
        }
    }
}
