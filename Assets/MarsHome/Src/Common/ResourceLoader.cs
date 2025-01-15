using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Suburb.Common
{
    public class ResourceLoader
    {
        private const string ROOT_PATH = "Prefabs";
        private const string RESOURCES = "Resources";
        
        public IObservable<GameObject[]> LoadFromResources(string folderPath)
        {
            string pathToPrefabsFolder = Path.Combine(ROOT_PATH, folderPath);
            string resourcesPath = Path.Combine(Application.dataPath, RESOURCES, pathToPrefabsFolder);
            string[] prefabFileNames = Directory.GetFiles(resourcesPath, "*.prefab");

            IObservable<GameObject>[] observables = new IObservable<GameObject>[prefabFileNames.Length];
                
            for (int i = 0; i < prefabFileNames.Length; i++)
            {
                string prefabFileName = prefabFileNames[i];
                string prefabName = Path.GetFileNameWithoutExtension(prefabFileName);
                observables[i] = LoadPrefabAsync(pathToPrefabsFolder, prefabName)
                    .ToObservable();
            }

            return Observable.WhenAll(observables);
        }

        private Task<GameObject> LoadPrefabAsync(string folderPath, string prefabName)
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>(folderPath + "/" + prefabName);
            return Task.FromResult(request.asset as GameObject);
        }

    }
}