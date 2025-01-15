using System;
using Suburb.Utils.Serialization;
using TMPro;
using UnityEngine;

namespace Suburb.ResourceMaps
{
    [Serializable]
    public class TextListAnimationData
    {
        [SerializeField] private TMP_Text[] texts;
        [SerializeField] private RectTransform[] textMasks;
        [SerializeField] private RectTransform buttonsBlock;
        
        public RectTransform ButtonsBlock => buttonsBlock;
        public TMP_Text[] Texts => texts;
        public RectTransform[] TextMasks => textMasks;
    }
}