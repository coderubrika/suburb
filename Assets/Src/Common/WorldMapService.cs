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
        private readonly List<GameObject> assetsList = new();
        
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
        }

        public void Generate()
        {
            gameResourcesData = savesService.SelectedData.GameResourcesData;

            SetupLand();
            SetupRovers();
        }

        public void Show()
        {
            assetsList.SetActiveGameObjects(true);
        }

        public void Hide()
        {
            assetsList.SetActiveGameObjects(false);
        }

        private void SetupLand()
        {
            GameObject landPrefab = resourcesRepository.Items.FirstOrDefault(item => item.name == "Land");
            GameObject landObject = injectCreator.Create(landPrefab, assetsRoot);
            landObject.SetActive(false);
            assetsList.Add(landObject);
        }

        private void SetupRovers()
        {
            AssetsCategory<RoverData> data = gameResourcesData.RoversData;
            RoverData[] roversDatas = data.AssetsList;

            GameObject roverPrefab  = resourcesRepository.Items.FirstOrDefault(item => item.name == data.Name);

            foreach(RoverData roverData in roversDatas)
            {
                GameObject roverObject = injectCreator.Create(roverPrefab, assetsRoot);

                roverObject.transform.position = roverData.Position.ToVector3();
                roverObject.transform.localRotation = Quaternion.Euler(roverData.Rotation.ToVector3());
                roverObject.SetActive(false);
                assetsList.Add(roverObject);
            }
        }
    }
}
