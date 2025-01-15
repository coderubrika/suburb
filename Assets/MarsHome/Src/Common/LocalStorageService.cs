using Newtonsoft.Json;
using Suburb.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using Suburb.Utils.Serialization;
using UnityEngine;

namespace Suburb.Common
{
    public class LocalStorageService
    {
        private readonly WebClientService webClientService;

        private readonly JsonSerializerSettings convertSettings;
        
        public LocalStorageService(WebClientService webClientService)
        {
            this.webClientService = webClientService;

            convertSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new Vector3Converter(),
                    new EaseConverter()
                }
            };
        }

        public void RemoveFile(string path)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, path);
            if (!File.Exists(fullPath))
                return;

            File.Delete(fullPath);
        }

        private IObservable<T> LoadFromStreaming<T>(string filePath)
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

        public void SaveToPersistent<T>(string filePath, T data)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, filePath);
            SaveTo(fullPath, data);
        }

        public void CreatePersistentFolder(string folderPath)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, folderPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
        }

        private void SaveTo<T>(string fullPath, T data)
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            bool isDebug = Application.isEditor || Debug.isDebugBuild;
            string json = JsonConvert.SerializeObject(data, isDebug ? Formatting.Indented : Formatting.None, convertSettings);

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
                return JsonConvert.DeserializeObject<T>(json, convertSettings);
            }
            catch (Exception ex)
            {
                this.LogError($"Parsing {typeof(T).Name} failed. Message: {ex.Message} Json: {json}");
                return default(T);
            }
        }
    }
}
