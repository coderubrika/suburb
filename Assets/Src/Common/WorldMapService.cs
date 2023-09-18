using Suburb.Serialization;
using Suburb.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Suburb.Common
{
    public class WorldMapService
    {
        private readonly ResourcesRepository resourcesRepository;
        private readonly SavesService savesService;
        private readonly InjectCreator injectCreator;

        private readonly Transform assetsRoot;
        private readonly Transform assetsPoolRoot;
        private readonly HashSet<GameObject> activeAssets = new();

        private readonly Dictionary<string, Stack<GameObject>> assetsPool = new();
        private readonly int poolStepCount = 1;

        private bool isPoolWarmedUp;
        private GameResourcesData gameResourcesData;

        public WorldMapService(
            ResourcesRepository resourcesRepository,
            SavesService savesService,
            InjectCreator injectCreator)
        {
            this.resourcesRepository = resourcesRepository;
            this.savesService = savesService;
            this.injectCreator = injectCreator;

            assetsRoot = new GameObject("AssetsRoot").transform;
            assetsPoolRoot = new GameObject("AssetsPoolRoot").transform;

            GameObject.DontDestroyOnLoad(assetsRoot.gameObject);
            GameObject.DontDestroyOnLoad(assetsPoolRoot.gameObject);
        }

        public void Generate()
        {
            if (!isPoolWarmedUp)
            {
                WarmUpPool();
                isPoolWarmedUp = true;
            }

            gameResourcesData = savesService.TmpData.GameResourcesData;

            SetupLand();
            SetupRovers();
        }

        public void Show()
        {
            activeAssets.SetActiveGameObjects(true);
        }

        public void Hide()
        {
            activeAssets.SetActiveGameObjects(false);
        }

        public void Clear()
        {
            if (!isPoolWarmedUp)
                return;

            foreach (var asset in activeAssets.ToArray())
            {
                Stack<GameObject> pool = assetsPool[asset.name];

                if (pool.Count >= poolStepCount)
                    Object.Destroy(asset);

                asset.SetActive(false);
                asset.transform.SetParent(assetsPoolRoot, true);
                pool.Push(asset);
            }

            activeAssets.Clear();

            foreach (Stack<GameObject> poop in assetsPool.Values)
                while (poop.Count > poolStepCount)
                    Object.Destroy(poop.Pop());
        }

        private void SetupLand()
        {
            GameObject landObject = GetFromPool("Land");
            landObject.SetActive(false);
            landObject.transform.SetParent(assetsRoot, true);
            activeAssets.Add(landObject);
        }

        private void SetupRovers()
        {
            AssetsCategory<RoverData> data = gameResourcesData.RoversData;
            RoverData[] roversDatas = data.AssetsList;

            foreach(RoverData roverData in roversDatas)
            {
                GameObject roverObject = GetFromPool(data.Name);
                roverObject.transform.SetParent(assetsRoot, true);
                roverObject.transform.position = roverData.Position.ToVector3();
                roverObject.transform.localRotation = Quaternion.Euler(roverData.Rotation.ToVector3());
                roverObject.SetActive(false);
                activeAssets.Add(roverObject);
            }
        }

        private GameObject GetFromPool(string name)
        {
            Stack<GameObject> poolStore = assetsPool[name];
            
            if (poolStore.Count == 0)
            {
                GameObject prefab = resourcesRepository.Items.FirstOrDefault(item => item.name == name);
                WarmUpPoolFor(prefab);
            }

            return poolStore.Pop();
        }

        private void WarmUpPool()
        {
            foreach (var prefab in resourcesRepository.Items)
            {
                assetsPool[prefab.name] = new();
                WarmUpPoolFor(prefab);
            }
        }

        private void WarmUpPoolFor(GameObject prefab)
        {
            for (int i = 0; i < poolStepCount; i++)
            {
                Stack<GameObject> pool = assetsPool[prefab.name];
                GameObject asset = injectCreator.Create(prefab, assetsPoolRoot);
                asset.SetActive(false);
                asset.name = prefab.name;
                pool.Push(asset);
            }
        }
    }
}
