using System.Collections.Generic;

namespace Suburb.Common
{
    public class SavesService
    {
        private readonly LocalStorageService localStorageService;

        private readonly Dictionary<string, GameCollectedData> saves = new();
        private readonly List<int> changedDatas = new();

        private static string SAVES_FOLDER_PATH = "saves";
        private static string SAVES_LIST_FILE_PATH = "savesList.json";

        public SavesService(LocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;

            string[] savesPathes = localStorageService.LoadFromPersistent<string[]>(string.Empty);
        }

        public int SavesCount { get => saves.Count; }

        public GameCollectedConfig Create()
        {
            var gameData = new GameCollectedData()
            {
                SaveTime = 0
            };

            GameCollectedConfig gameConfig = new(gameData);
            return gameConfig;
        }

        public GameCollectedConfig GetLast()
        {
            return null;
        }

        public void Select(int configId)
        {

        }

        public void Update(int configId)
        {

        }
    }
}
