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
        [SerializeField] private TransformData startupCameraTransform;
        [SerializeField] private TransformData startupProbeTransform;
        [SerializeField] private TransformData startupCameraCloseStartTransform;
        [SerializeField] private TransformData startupCameraCloseEndTransform;
        [SerializeField] private TransformData startupProbeCloseStartTransform;
        [SerializeField] private TransformData startupProbeCloseEndTransform;
        [SerializeField] private AnimationSettingsData startCenterAnim;
        [SerializeField] private AnimationSettingsData regularCenterAnim;
        [SerializeField] private AnimationSettingsData rightSideAnim;
        [SerializeField] private AnimationSettingsData startupCameraAnim;
        [SerializeField] private AnimationSettingsData startupProbeAnim;
        [SerializeField] private AnimationSettingsData startupCameraCloseEndAnim;
        [SerializeField] private AnimationSettingsData startupProbeCloseEndAnim;
        
        public TransformData HideTransform => hideTransform;
        public TransformData CenterTransform => centerTransform;
        public TransformData RightTransform => rightTransform;
        public AnimationSettingsData StartCenterAnim => startCenterAnim;
        public AnimationSettingsData RegularCenterAnim => regularCenterAnim;
        public AnimationSettingsData RightSideAnim => rightSideAnim;
        public TransformData StartupCameraTransform => startupCameraTransform;
        public TransformData StartupProbeTransform => startupProbeTransform;
        public AnimationSettingsData StartupCameraAnim => startupCameraAnim;
        public AnimationSettingsData StartupProbeAnim => startupProbeAnim;
        public TransformData StartupCameraCloseStartTransform => startupCameraCloseStartTransform;
        public TransformData StartupCameraCloseEndTransform => startupCameraCloseEndTransform;
        public AnimationSettingsData StartupCameraCloseEndAnim => startupCameraCloseEndAnim;
        public TransformData StartupProbeCloseStartTransform => startupProbeCloseStartTransform;
        public TransformData StartupProbeCloseEndTransform => startupProbeCloseEndTransform;
        public AnimationSettingsData StartupProbeCloseEndAnim => startupProbeCloseEndAnim;
    }
}