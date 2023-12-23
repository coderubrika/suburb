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
        }

        private void StandCamera(TransformData transformData)
        {
            camera.transform.position = transformData.Position;
            camera.transform.localRotation = Quaternion.Euler(transformData.Rotation);
        }
        
        private void Hide()
        {
            root.gameObject.SetActive(false);
        }

        private Tween AnimateTo(ValueAnimationData<TransformData> animationTransformData)
        {
            TransformData transformData = animationTransformData.End;
            AnimationSettingsData animationData = animationTransformData.AnimationSettings;
            
            Sequence sequence = DOTween.Sequence()
                .Append(camera.transform.DORotate(transformData.Rotation, animationData.Duration).SetEase(animationData.Easing))
                .Join(camera.transform.DOMove(transformData.Position, animationData.Duration).SetEase(animationData.Easing));
            return sequence;
        }
        
        public Tween AnimateEnterFirst()
        {
            Show();
            StandCamera(config.HideTransform);
            return AnimateTo(config.StartNewAnimationData);
        }
        
        public Tween AnimateEnter()
        {
            Show();
            return AnimateTo(config.StartNewAnimationData);
        }
        
        public Tween AnimateRight()
        {
            Show();
            return AnimateTo(config.RightSideAnimationData);
        }

        public void Initialize()
        {
            var sceneRefPrefab = resourcesService.GetPrefab("Menu3DScene");
            var sceneRef = injectCreator.Create(sceneRefPrefab, null);
            root = sceneRef.transform;
            camera = sceneRef.Refs["Camera"] as Camera;
            config = sceneRef.Refs["MenuSceneConfig"] as MenuSceneConfig;
            Hide();
        }
    }
}