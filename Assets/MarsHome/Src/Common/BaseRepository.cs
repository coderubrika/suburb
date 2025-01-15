using UnityEngine;

namespace Suburb.Common
{
    public class BaseRepository<T> : ScriptableObject
    {
        [SerializeField] private T[] items;

        public T[] Items { get => items; }
    }
}
