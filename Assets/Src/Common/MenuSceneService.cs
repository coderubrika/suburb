using System;
using DG.Tweening;
using Suburb.Utils;
using Suburb.Utils.Serialization;
using UniRx;
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
        private Sequence probeSequence;
        private MenuMarsController menuMarsController;
        private GameObject probeObject;
        
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
        
        public void Hide()
        {
            probeSequence?.Kill();
            cameraSequence?.Kill();
            root.gameObject.SetActive(false);
            menuMarsController.Hide();
        }
        
        private IObservable<Unit> PlayTo(Transform transform, ref Sequence sequence, TransformData transformData, AnimationSettingsData animationData)
        {
            Subject<Unit> onKill = new();
            sequence?.Kill();
            sequence = DOTween.Sequence()
                .AppendInterval(animationData.Delay)
                .Append(
                    transform.DORotate(transformData.Rotation, animationData.Duration).SetEase(animationData.Easing))
                .Join(transform.DOMove(transformData.Position, animationData.Duration).SetEase(animationData.Easing))
                .OnKill(() =>
                {
                    onKill.OnNext(Unit.Default);
                    onKill.OnCompleted();
                });
            return onKill;
        }
        
        public void PlayEnterFirst()
        {
            Show();
            StandCamera(config.HideTransform);
            PlayTo(camera.transform, ref cameraSequence, config.CenterTransform, config.StartCenterAnim);
        }
        
        public void AnimateEnter()
        {
            Show();
            PlayTo(camera.transform, ref cameraSequence, config.CenterTransform, config.RegularCenterAnim);
        }
        
        public void AnimateRight()
        {
            Show();
            PlayTo(camera.transform, ref cameraSequence, config.RightTransform, config.RightSideAnim);
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
            probeObject = refs["Probe"] as GameObject;
            probeObject.SetActive(false);
            menuMarsController = injectCreator.Create<MenuMarsController>(root, mars);
            Hide();
        }

        public IObservable<Unit> AnimateStartup()
        {
            cameraSequence?.Kill();
            menuMarsController.Pause();
            StandCamera(config.CenterTransform);
            probeObject.SetActive(true);
            PlayTo(camera.transform, ref cameraSequence, config.StartupCameraTransform, config.StartupCameraAnim);
            return PlayTo(probeObject.transform, ref probeSequence, config.StartupProbeTransform, config.StartupProbeAnim)
                .ContinueWith(() =>
                {
                    StandCamera(config.StartupCameraCloseStartTransform);
                    return PlayTo(config.StartupCameraCloseEndTransform);
                });
        }
    }
}