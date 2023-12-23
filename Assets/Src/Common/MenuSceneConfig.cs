using Suburb.Utils.Serialization;
using UnityEngine;

namespace Suburb.Common
{
    [CreateAssetMenu(fileName = "MenuSceneConfig", menuName = "Configs/MenuSceneConfig")]
    
    public class MenuSceneConfig : ScriptableObject
    {
        [SerializeField] private TransformData hideTransform;
        [SerializeField] private TransformData centerTransform;
        [SerializeField] private TransformData rightTransform;
        [SerializeField] private AnimationSettingsData startCenterAnim;
        [SerializeField] private AnimationSettingsData regularCenterAnim;
        [SerializeField] private AnimationSettingsData rightSideAnim;
        
        public TransformData HideTransform => hideTransform;
        public TransformData CenterTransform => centerTransform;
        public TransformData RightTransform => rightTransform;
        public AnimationSettingsData StartCenterAnim => startCenterAnim;
        public AnimationSettingsData RegularCenterAnim => regularCenterAnim;
        public AnimationSettingsData RightSideAnim => rightSideAnim;
    }
}