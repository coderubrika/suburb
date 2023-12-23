using Suburb.Serialization;
using Suburb.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Suburb.Common
{
    public class WorldMapService
    {
        private readonly PrefabsRepository prefabsRepository;
        private readonly SavesService savesService;
        private readonly InjectCreator injectCreator;

        private bool isPoolWarmedUp;
        private GameResourcesData gameResourcesData;

        public WorldMapService(
            PrefabsRepository prefabsRepository,
            SavesService savesService,
            InjectCreator injectCreator)
        {
            this.prefabsRepository = prefabsRepository;
            this.savesService = savesService;
            this.injectCreator = injectCreator;
        }

        public void Generate()
        {
        }

        public void Show()
        {
        }

        public void Hide()
        {
        }

        public void Clear()
        {
        }
    }
}
