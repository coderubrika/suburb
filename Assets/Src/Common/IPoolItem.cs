using System.Collections.Generic;
using UnityEngine;

namespace Suburb.Common
{
    public interface IPoolItem
    {
        public Dictionary<string, object> Parameters { get; }
        public void Spawn();
        public void Despawn();
    }
}