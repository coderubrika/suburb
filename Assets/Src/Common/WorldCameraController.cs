using Suburb.Core;
using Suburb.Core.Inputs;
using Suburb.Utils;
using System;
using UniRx;
using UnityEngine;

namespace Suburb.Common
{
    public class WorldCameraController : IDisposable
    {
        private readonly Transform cameraTransform;
        private readonly IGestureProvider gestureProvider;

        private readonly CompositeDisposable disposables = new();

        // TODO move to project settings
        private readonly float zoomToScaleFactor = 5f;
        private readonly float maxScale = 50f;
        private readonly float minScale = -14f;

        private float currentScale = 1f;
        private float originaTransformPositionY;

        private IDisposable dragDisposable;
        private bool isOn;
        private Vector2 deltaPositon;
        private Vector3 velocity = Vector3.zero;
        private SmoothTransitionParam smoothTransitionParam;
        private Vector3 oldPosition;
        private bool isAllowToDisposeDrag;

        public WorldCameraController(
            PlayerCamera playerCamera, 
            IGestureProvider gestureProvider, 
            SmoothTransitionParam smoothTransitionParam)
        {
            cameraTransform = playerCamera.transform;
            this.gestureProvider = gestureProvider;
            this.smoothTransitionParam = smoothTransitionParam;
            originaTransformPositionY = cameraTransform.position.y;
        }

        public void Enable()
        {
            if (isOn)
                return;

            isOn = true;

            gestureProvider.OnDrag
                .Subscribe(data =>
                {
                    deltaPositon = data.Delta;
                }).AddTo(disposables);

            gestureProvider.OnDragStart
                .Subscribe(_ =>
                {
                    dragDisposable?.Dispose();
                    velocity = Vector3.zero;
                    isAllowToDisposeDrag = false;
                    dragDisposable = Observable.EveryUpdate()
                        .Subscribe(UpdateMove)
                        .AddTo(disposables);
                }).AddTo(disposables);

            gestureProvider.OnDragEnd
                .Subscribe(_ =>
                {
                    deltaPositon = Vector3.zero;
                    isAllowToDisposeDrag = true;
                }).AddTo(disposables);

            gestureProvider.OnZoom.
                Subscribe(UpdateScale)
                .AddTo(disposables);
        }

        public void Disable()
        {
            if (!isOn)
                return;

            isOn = false;

            disposables.Clear();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        private void UpdateMove(long _)
        {
            Vector3 newPosition = Vector3.SmoothDamp(
                cameraTransform.position,
                cameraTransform.position
                    - new Vector3(deltaPositon.x, 0f, deltaPositon.y)
                    * smoothTransitionParam.MoveSpeed, ref velocity, smoothTransitionParam.SmoothTime);

            if (isAllowToDisposeDrag && newPosition.IsCloseWithOther(oldPosition, float.Epsilon))
            {
                velocity = Vector3.zero;
                dragDisposable?.Dispose();
            }

            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                cameraTransform.position
                    - new Vector3(deltaPositon.x, 0f, deltaPositon.y)
                    * smoothTransitionParam.MoveSpeed, ref velocity, smoothTransitionParam.SmoothTime);

            oldPosition = newPosition;
        }

        private void UpdateScale(GestureEventData data)
        {
            float oldScale = currentScale;
            currentScale = Mathf.Clamp(currentScale - data.ZoomDelta.y * zoomToScaleFactor, minScale, maxScale);
            this.Log($"currentScale: {currentScale}");
            float realScale = currentScale - oldScale;

            Vector3 currentPosition = cameraTransform.position;
            Vector3 newPosition = currentPosition - cameraTransform.forward * realScale;

            cameraTransform.position = newPosition;
        }
    }
}
