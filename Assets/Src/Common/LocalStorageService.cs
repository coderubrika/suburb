using Newtonsoft.Json;
using Suburb.Utils;
using System;
using System.IO;
using UnityEngine;

namespace Suburb.Common
{
    public class LocalStorageService
    {
        private readonly WebClientService webClientService;

        public LocalStorageService(WebClientService webClientService)
        {
            this.webClientService = webClientService;
        }

        public void RemoveFile(string path)
        {
            if (!File.Exists(path))
                return;

            File.Delete(path);
        }

        public IObservable<T> LoadFromStreaming<T>(string filePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, filePath);
            this.Log($"Load from streaming {fullPath}");

            if (Application.platform == RuntimePlatform.Android)
                return webClientService.GetViaJson<T>(fullPath, null, null, null, false);

            return GeneralUtils.StartWithDefault(LoadFrom<T>(fullPath));
        }

        public T LoadFromPersistent<T>(string filePath)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, filePath);
            return LoadFrom<T>(fullPath);
        }

        private void SaveToStreaming<T>(string filePath, T data)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, filePath);
            SaveTo(fullPath, data);
        }

        private void SaveToPersistent<T>(string filePath, T data)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, filePath);
            SaveTo(fullPath, data);
        }

        private void SaveTo<T>(string fullPath, T data)
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            bool isDebug = Application.isEditor || Debug.isDebugBuild;
            string json = JsonConvert.SerializeObject(data, isDebug ? Formatting.Indented : Formatting.None);

            File.WriteAllText(fullPath, json);
        }

        private T LoadFrom<T>(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                this.Log($"Can't load {fullPath}. File doesn't exist.");
                return default(T);
            }

            string json = File.ReadAllText(fullPath);
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                this.LogError($"Parsing {typeof(T).Name} failed. Message: {ex.Message} Json: {json}");
                return default(T);
            }
        }
    }
}
