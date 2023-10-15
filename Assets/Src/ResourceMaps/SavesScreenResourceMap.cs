using System;
using Suburb.Common;
using UnityEngine;

namespace Suburb.ResourceMaps
{
    [Serializable]
    public class SavesScreenResourceMap : IResourceMap
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public CanvasGroup CanvasGroup => canvasGroup;
    }
}