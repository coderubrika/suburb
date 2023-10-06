using System.Collections.Generic;
using UnityEngine;

namespace Suburb.Common
{
    public class AssetsGroupPool<TComponent>
        where TComponent : Component, IPoolItem
    {
        private readonly ResourcesService resourcesService;
        private readonly AssetsPool<TComponent>[] pools;

        private const string GROUP_POOL_IDX = "groupPoolIdx";
        
        public AssetsGroupPool(ResourcesService resourcesService, TComponent[] prototypes)
        {
            this.resourcesService = resourcesService;
            pools = new AssetsPool<TComponent>[prototypes.Length];

            for (int i = 0; i < prototypes.Length; i++)
            {
                prototypes[i].Parameters[GROUP_POOL_IDX] = i;
                pools[i] = this.resourcesService.GetPool(prototypes[i]);
            }
        }

        public TComponent Spawn(int idx)
        {
            idx = Mathf.Clamp(idx, 0, pools.Length - 1);
            return pools[idx].Spawn();
        }

        public bool Despawn(TComponent item)
        {
            if (!item.Parameters.TryGetValue(GROUP_POOL_IDX, out object idx))
                return false;
            
            int iIdx = (int)idx;
            iIdx = Mathf.Clamp(iIdx, 0, pools.Length - 1);
            return pools[iIdx].Despawn(item);
        }
    }
}