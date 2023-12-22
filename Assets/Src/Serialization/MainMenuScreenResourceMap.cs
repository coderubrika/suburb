using System;
using DG.Tweening;
using Suburb.Common;
using Suburb.Utils.Serialization;
using TMPro;
using UnityEngine;

namespace Suburb.ResourceMaps
{
    [Serializable]
    public class MainMenuScreenResourceMap
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private AnimationSettingsData cameraAnimSettings;
        [SerializeField] private TransformData cameraStart;
        [SerializeField] private TransformData cameraEnd;
        [SerializeField] private TMP_Text[] texts;
        [SerializeField] private RectTransform[] textMasks;
        [SerializeField] private RectTransform buttonsBlock;

        public RectTransform ButtonsBlock => buttonsBlock;

        public CanvasGroup CanvasGroup => canvasGroup;

        public AnimationSettingsData CameraAnimSettings => cameraAnimSettings;

        public TransformData CameraStart => cameraStart;

        public TransformData CameraEnd => cameraEnd;
        
        public TMP_Text[] Texts => texts;

        public RectTransform[] TextMasks => textMasks;
    }
}