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
        private readonly Vector3 cameraForward;
        private readonly Vector3 initialCameraPosition;
        private readonly CompositeDisposable disposables = new();

        // TODO move to project settings
        private readonly float zoomFactor = 5f;
        private readonly float maxZoom = 15f;
        private readonly float minZoom = -15f;
        private readonly float maxMoveSensivityFactor = 3f;
        private readonly float minMoveSensivity = 0.3333f;

        private float currentZoom = 0f;
        // i need keep real camera position equal 0 current zoom
        private float originaTransformPositionY;

        private IDisposable dragDisposable;
        private bool isOn;
        private Vector2 deltaPositon;
        private Vector3 velocity = Vector3.zero;
        private SmoothTransitionParam smoothTransitionParam;
        private Vector3 oldPosition;
        private bool isAllowToDisposeDrag;
        private int currentPointerId = -1;

        public WorldCameraController(
            PlayerCamera playerCamera, 
            IGestureProvider gestureProvider, 
            SmoothTransitionParam smoothTransitionParam)
        {
            cameraTransform = playerCamera.transform;
            cameraForward = cameraTransform.forward;
            initialCameraPosition = cameraTransform.position;
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
                    if (data.Id != currentPointerId)
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
            float zoomDelta = data.ZoomDelta.y * zoomFactor;
            currentZoom += zoomDelta;

            if (currentZoom > maxZoom || currentZoom < minZoom)
            {
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                return;
            }

            cameraTransform.position += cameraForward * zoomDelta;
        }

        private float GetMoveSpeed()
        {
            float currentZoomNormalized = Mathf.InverseLerp(minZoom, maxZoom, currentZoom);
            float currentZoomNormalizedInversed = 1 - currentZoomNormalized;
            float currentMoveSpeedFactor = Mathf.Lerp(minMoveSensivity, maxMoveSensivityFactor, currentZoomNormalizedInversed);
            float currentMoveSpeed = smoothTransitionParam.MoveSpeed * currentMoveSpeedFactor;
            return currentMoveSpeed;
        }

        private void ClearLastDrag()
        {
            dragDisposable?.Dispose();
            velocity = Vector3.zero;
            isAllowToDisposeDrag = false;
        }
    }
}
