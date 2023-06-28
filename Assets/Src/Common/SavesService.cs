using Suburb.Utils;
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
        private readonly HashSet<string> savesFileNames;

        private static string SAVES_FOLDER = "saves";
        private static string SAVES_DATA_PATH = Path.Combine(SAVES_FOLDER, "savesData.json");

        public GameCollectedData SelectedData { get; private set; }

        public SavesService(
            LocalStorageService localStorageService,
            GameSettingsRepository gameSettingsRepository)
        {
            this.localStorageService = localStorageService;
            this.gameSettingsRepository = gameSettingsRepository;

            var savesData = localStorageService.LoadFromPersistent<SavesData>(SAVES_DATA_PATH);
            savesFileNames = savesData == null || savesData.FileNames == null ? new() : savesData.FileNames.ToHashSet();

            saves = savesFileNames
                .Select(fileName =>
                {
                    var data = localStorageService
                        .LoadFromPersistent<GameCollectedData>(Path.Combine(SAVES_FOLDER, fileName));
                    return data;
                })
                .ToDictionary(item => item.UID, item => item);

            if (savesData == null || string.IsNullOrEmpty(savesData.LastSaveUID))
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

            savesFileNames.Add(SelectedData.FileName);
            localStorageService.SaveToPersistent(Path.Combine(SAVES_FOLDER, SelectedData.FileName), SelectedData);
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
            savesFileNames.Remove(uid);
            localStorageService.RemoveFile(Path.Combine(SAVES_FOLDER, deletingData.FileName));
        }

        public void SyncData()
        {
            localStorageService.SaveToPersistent(
                SAVES_DATA_PATH,
                new SavesData { FileNames = savesFileNames.ToArray(), LastSaveUID = SelectedData.UID });
        }

        public void Rename(string uid, string name)
        {
            saves[uid].Rename(name);
        }
    }
}
