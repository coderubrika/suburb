using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Suburb.UI
{
    public class ItemWithButton<T> : MonoBehaviour
    {
        [SerializeField] private Button button;

        public T Item { get; private set; }
        public Button Button { get => button; }

        [Inject]
        public void Construct(T Item)
        {
            this.Item = Item;
        }
    }
}
