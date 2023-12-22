using DG.Tweening;
using Suburb.Utils.Serialization;
using UnityEngine;
using Zenject;

namespace Suburb.Common
{
    public class MenuSceneService : IInitializable
    {
        private readonly ResourcesService resourcesService;
        private readonly Camera camera;
        private readonly AnimationSettingsData cameraAnim;
        private readonly TransformData cameraStart;
        private readonly TransformData cameraEnd;
        
        private const string ROOT_NAME = "MenuScreneRoot";
        
        private Transform root;
        private Mars mars;
        
        public MenuSceneService(
            ResourcesService resourcesService, 
            Camera camera,
            ValueAnimationData<TransformData> config)
        {
            this.resourcesService = resourcesService;
            this.camera = camera;
            cameraAnim = config.AnimationSettings;
            cameraStart = config.Start;
            cameraEnd = config.End;
        }
        
        public void Show()
        {
            mars.gameObject.SetActive(true);
        }

        public void StandCameraToStart()
        {
            camera.transform.position = cameraStart.Position;
            camera.transform.localRotation = Quaternion.Euler(cameraStart.Rotation);
        }

        public void StandCameraToEnd()
        {
            camera.transform.position = cameraEnd.Position;
            camera.transform.localRotation = Quaternion.Euler(cameraEnd.Rotation);
        }
        
        public void Hide()
        {
            mars.gameObject.SetActive(false);
        }

        public Sequence BindAnimation(Sequence sequence)
        {
            StandCameraToStart();
            sequence
                .Append(camera.transform.DORotate(cameraEnd.Rotation, cameraAnim.Duration).SetEase(cameraAnim.Easing))
                .Join(camera.transform.DOMove(cameraEnd.Position, cameraAnim.Duration).SetEase(cameraAnim.Easing));
            return sequence;
        }

        public void Initialize()
        {
            mars = resourcesService.GetInstance<Mars>("Mars");
            root = new GameObject(ROOT_NAME).transform;
            Object.DontDestroyOnLoad(root.gameObject);
            mars.transform.SetParent(root);
        }
    }
}