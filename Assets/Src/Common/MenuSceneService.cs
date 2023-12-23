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
        private readonly AnimationSettingsData cameraAnim;
        private readonly TransformData cameraStart;
        private readonly TransformData cameraEnd;

        private Transform root;
        private GameObject marsObject;
        private Camera camera;
        
        public MenuSceneService(
            ResourcesService resourcesService, 
            InjectCreator injectCreator,
            ValueAnimationData<TransformData> config)
        {
            this.resourcesService = resourcesService;
            this.injectCreator = injectCreator;
            cameraAnim = config.AnimationSettings;
            cameraStart = config.Start;
            cameraEnd = config.End;
        }
        
        private void Show()
        {
            root.gameObject.SetActive(true);
        }

        private void StandCameraToStart()
        {
            camera.transform.position = cameraStart.Position;
            camera.transform.localRotation = Quaternion.Euler(cameraStart.Rotation);
        }

        private void StandCameraToEnd()
        {
            camera.transform.position = cameraEnd.Position;
            camera.transform.localRotation = Quaternion.Euler(cameraEnd.Rotation);
        }
        
        private void Hide()
        {
            root.gameObject.SetActive(false);
        }

        public Sequence BindAnimation(Sequence sequence)
        {
            Show();
            StandCameraToStart();
            sequence
                .Append(camera.transform.DORotate(cameraEnd.Rotation, cameraAnim.Duration).SetEase(cameraAnim.Easing))
                .Join(camera.transform.DOMove(cameraEnd.Position, cameraAnim.Duration).SetEase(cameraAnim.Easing))
                .OnKill(StandCameraToEnd);
            return sequence;
        }

        public void Initialize()
        {
            var sceneRefPrefab = resourcesService.GetPrefab("Menu3DScene");
            var sceneRef = injectCreator.Create(sceneRefPrefab, null);
            root = sceneRef.transform;
            camera = sceneRef.Refs["Camera"] as Camera;
            Hide();
        }
    }
}