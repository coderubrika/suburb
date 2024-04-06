using Suburb.Core;
using Suburb.Inputs;
using Suburb.Serialization;
using Suburb.Utils;
using System;
using Suburb.Cameras;
using UniRx;
using UnityEngine;

namespace Suburb.Common
{
    public class WorldCameraController : IDisposable
    {
        private readonly IGestureProvider gestureProvider;
        private readonly CameraService cameraService;
        private readonly GameSettingsRepository gameSettingsRepository;
        private readonly SavesService savesService;

        private readonly Transform cameraTransform;
        private readonly Vector3 cameraForward;
        private readonly CompositeDisposable disposables = new();
        private readonly WorldCameraControllerSettings settings;

        private float currentZoom;

        private IDisposable dragDisposable;
        private bool isOn;
        private Vector2 deltaPositon;
        private Vector3 velocity = Vector3.zero;
        private SmoothTransitionParam smoothTransitionParam;
        private Vector3 oldPosition;
        private bool isAllowToDisposeDrag;
        private int currentPointerId = -1;
        private bool isPinchDragEnabled;

        public WorldCameraController(
            CameraService cameraService, 
            IGestureProvider gestureProvider,
            GameSettingsRepository gameSettingsRepository,
            SavesService savesService,
            Camera camera)
        {
            this.gestureProvider = gestureProvider;
            this.cameraService = cameraService;
            this.gameSettingsRepository = gameSettingsRepository;
            this.savesService = savesService;

            Camera = camera;
            settings = gameSettingsRepository.WorldCameraControllerSettings;
            smoothTransitionParam = settings.SmoothTransitionParam;
            cameraTransform = camera.transform;
            cameraForward = cameraTransform.forward;
        }
        
        public Camera Camera { get; }
        public void Enable()
        {
            if (isOn)
                return;

            var data = savesService.TmpData.WorldCameraControllerData;
            currentZoom = data.Zoom;
            cameraTransform.position = data.Position.ToVector3();

            isOn = true;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                SubscribeOnMobileEvents();

            SubscribeOnGeneralEvents();
        }

        public void Disable()
        {
            if (!isOn)
                return;

            savesService.TmpData.UpdateWorldCameraControllerData(new WorldCameraControllerData
            {
                Zoom = currentZoom,
                Position = cameraTransform.position.ToVector3Data(),
            });

            isOn = false;

            disposables.Clear();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        private void UpdateMove(long _)
        {
            float currentMoveSpeed = GetMoveSpeed();
            Vector3 newPosition = Vector3.SmoothDamp(
                cameraTransform.position,
                cameraTransform.position
                    - new Vector3(deltaPositon.x, 0f, deltaPositon.y)
                    * currentMoveSpeed, ref velocity, smoothTransitionParam.SmoothTime);

            if (isAllowToDisposeDrag && newPosition.IsCloseWithOther(oldPosition, float.Epsilon))
            {
                currentPointerId = -1;
                velocity = Vector3.zero;
                dragDisposable?.Dispose();
                return;
            }

            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                cameraTransform.position
                    - new Vector3(deltaPositon.x, 0f, deltaPositon.y)
                    * currentMoveSpeed, ref velocity, smoothTransitionParam.SmoothTime);

            oldPosition = newPosition;
        }

        private void UpdateScale(GestureEventData data)
        {
            // float zoomDelta = data.ZoomDelta.y * settings.ZoomFactor;
            // currentZoom += zoomDelta;
            //
            // if (currentZoom > settings.MaxZoom || currentZoom < settings.MinZoom)
            // {
            //     currentZoom = Mathf.Clamp(currentZoom, settings.MinZoom, settings.MaxZoom);
            //     return;
            // }
            //
            // cameraTransform.position += cameraForward * zoomDelta;
        }

        private float GetMoveSpeed()
        {
            float currentZoomNormalized = Mathf.InverseLerp(settings.MinZoom, settings.MaxZoom, currentZoom);
            float currentZoomNormalizedInversed = 1 - currentZoomNormalized;
            float currentMoveSpeedFactor = Mathf.Lerp(settings.MinMoveSensivity, settings.MaxMoveSensivityFactor, currentZoomNormalizedInversed);
            float currentMoveSpeed = smoothTransitionParam.MoveSpeed * currentMoveSpeedFactor;
            return currentMoveSpeed;
        }

        private void ClearLastDrag()
        {
            dragDisposable?.Dispose();
            velocity = Vector3.zero;
            isAllowToDisposeDrag = false;
        }

        private void SubscribeOnMobileEvents()
        {
            if (gestureProvider is not TouchGestureProvider touchGestureProvider)
                return;

            touchGestureProvider.OnDragStartWithDoubleTouch
                .Subscribe(_ => isPinchDragEnabled = true)
                .AddTo(disposables);

            touchGestureProvider.OnDragEndWithDoubleTouch
                .Subscribe(_ => isPinchDragEnabled = false)
                .AddTo(disposables);

            touchGestureProvider.OnDragWithDoubleTouch
                .Subscribe(data =>
                {
                    if (!isPinchDragEnabled)
                        return;

                    deltaPositon = data.Delta;
                }).AddTo(disposables);
        }

        private void SubscribeOnGeneralEvents()
        {
            gestureProvider.OnDrag
                .Subscribe(data =>
                {
                    if (data.Id != currentPointerId || isPinchDragEnabled)
                        return;

                    deltaPositon = data.Delta;
                }).AddTo(disposables);

            gestureProvider.OnDragStart
                .Subscribe(data =>
                {
                    currentPointerId = data.Id;

                    ClearLastDrag();

                    dragDisposable = Observable.EveryUpdate()
                        .Subscribe(UpdateMove)
                        .AddTo(disposables);
                }).AddTo(disposables);

            gestureProvider.OnDragEnd
                .Subscribe(data =>
                {
                    if (currentPointerId != data.Id)
                        return;

                    deltaPositon = Vector3.zero;
                    isAllowToDisposeDrag = true;
                }).AddTo(disposables);

            // gestureProvider.OnZoom.
            //     Subscribe(UpdateScale)
            //     .AddTo(disposables);
        }
    }
}
