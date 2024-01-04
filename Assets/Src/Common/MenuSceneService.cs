using DG.Tweening;
using Suburb.Utils;
using Suburb.Utils.Serialization;
using UnityEngine;
using Zenject;

namespace Suburb.Common
{
    public class MenuSceneService : IInitializable
    {
        private readonly ResourcesService resourcesService;
        private readonly InjectCreator injectCreator;
        
        private MenuSceneConfig config;
        private Transform root;
        private GameObject marsObject;
        private Camera camera;
        private Sequence cameraSequence;
        private MenuMarsController menuMarsController;
        
        public MenuSceneService(
            ResourcesService resourcesService, 
            InjectCreator injectCreator)
        {
            this.resourcesService = resourcesService;
            this.injectCreator = injectCreator;
        }
        
        private void Show()
        {
            root.gameObject.SetActive(true);
            menuMarsController.Show();
        }

        private void StandCamera(TransformData transformData)
        {
            camera.transform.position = transformData.Position;
            camera.transform.localRotation = Quaternion.Euler(transformData.Rotation);
        }
        
        private void Hide()
        {
            root.gameObject.SetActive(false);
            menuMarsController.Hide();
        }

        private void AnimateTo(ValueAnimationData<TransformData> animationTransformData)
        {
            AnimateTo(animationTransformData.End, animationTransformData.AnimationSettings);
        }
        
        private void AnimateTo(TransformData transformData, AnimationSettingsData animationData)
        {
            cameraSequence?.Kill();
            cameraSequence = DOTween.Sequence()
                .Append(camera.transform.DORotate(transformData.Rotation, animationData.Duration).SetEase(animationData.Easing))
                .Join(camera.transform.DOMove(transformData.Position, animationData.Duration).SetEase(animationData.Easing));
        }
        
        public void AnimateEnterFirst()
        {
            Show();
            StandCamera(config.HideTransform);
            AnimateTo(config.CenterTransform, config.StartCenterAnim);
        }
        
        public void AnimateEnter()
        {
            Show();
            AnimateTo(config.CenterTransform, config.RegularCenterAnim);
        }
        
        public void AnimateRight()
        {
            Show();
            AnimateTo(config.RightTransform, config.RightSideAnim);
        }

        public void Initialize()
        {
            var sceneRefPrefab = resourcesService.GetPrefab("Menu3DScene");
            var sceneRef = injectCreator.Create(sceneRefPrefab, null);
            root = sceneRef.transform;
            var refs = sceneRef.Refs;
            camera = refs["Camera"] as Camera;
            config = refs["MenuSceneConfig"] as MenuSceneConfig;
            var mars = refs["Mars"] as GameObject;
            menuMarsController = injectCreator.Create<MenuMarsController>(root, mars);
            Hide();
        }
    }
}