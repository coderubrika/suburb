using Suburb.Utils.Serialization;
using UnityEngine;

namespace Suburb.Common
{
    [CreateAssetMenu(fileName = "MenuSceneConfig", menuName = "Configs/MenuSceneConfig")]
    
    public class MenuSceneConfig : ScriptableObject
    {
        [SerializeField] private TransformData hideTransform;
        [SerializeField] private ValueAnimationData<TransformData> startNewAnimationData;
        [SerializeField] private ValueAnimationData<TransformData> startContinueAnimationData;
        [SerializeField] private ValueAnimationData<TransformData> rightSideAnimationData;

        public TransformData HideTransform => hideTransform;

        public ValueAnimationData<TransformData> StartNewAnimationData => startNewAnimationData;

        public ValueAnimationData<TransformData> StartContinueAnimationData => startContinueAnimationData;

        public ValueAnimationData<TransformData> RightSideAnimationData => rightSideAnimationData;
    }
}