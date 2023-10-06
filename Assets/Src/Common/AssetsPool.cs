using System.Collections.Generic;
using Suburb.Utils;
using Suburb.Utils.Serialization;
using UnityEngine;

namespace Suburb.Common
{
    public class AssetsPool<TComponent>
        where TComponent : Component, IPoolItem
    {
        private readonly InjectCreator injectCreator;
        private readonly TComponent prototype;
        private readonly Transform root;
        private readonly Queue<TComponent> queue = new();
        private readonly string poolName;

        private const string POOL_NAME = "pool_name";
        
        public AssetsPool(
            InjectCreator injectCreator,
            TComponent prototype, 
            Transform root,
            string poolName)
        {
            this.injectCreator = injectCreator;
            this.prototype = prototype;
            this.root = root;
            this.poolName = poolName;
        }

        public TComponent Spawn(bool isActiveSelf = true, Transform parent = null, TransformData transformData = null)
        {
            var item = queue.Count > 0 ? queue.Dequeue() : CreateItem();
            
            if (parent != null)
                item.transform.SetParent(parent);
            
            if (transformData != null)
                item.transform.SetLocal(transformData);
            
            item.gameObject.SetActive(isActiveSelf);
            
            item.Spawn();
            return item;
        }

        public bool Despawn(TComponent item)
        {
            if (!item.Parameters.HasAndEqual(POOL_NAME, poolName))
                return false;
            
            item.Despawn();
            item.gameObject.SetActive(false);
            item.transform.ResetLocal();
            item.transform.SetParent(root);
            queue.Enqueue(item);

            return true;
        }

        public void Allocate(int count)
        {
            for (int i = 0; i < count; i++)
                queue.Enqueue(CreateItem());
        }

        public void Fit()
        {
            queue.DestroyGameObjects();
        }
        
        private TComponent CreateItem()
        {
            TComponent newItem = injectCreator.Create<TComponent>(prototype.gameObject, root);
            newItem.Parameters[POOL_NAME] = poolName;
            newItem.gameObject.SetActive(false);
            newItem.transform.ResetLocal();
            return newItem;
        }
    }
}