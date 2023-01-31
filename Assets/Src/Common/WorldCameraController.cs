using Suburb.Core;
using Suburb.Utils;
using System;
using UniRx;
using UnityEngine;

namespace Suburb.Common
{
    public class WorldCameraController : IDisposable
    {
        private readonly Transform cameraTransform;
        private readonly PointerService pointerService;

        private readonly CompositeDisposable disposables = new();

        private IDisposable dragDisposable;
        private bool isOn;
        private Vector3 startPosition;
        private Vector2 deltaPositon;
        private Vector3 velocity = Vector3.zero;
        private SmoothTransitionParam smoothTransitionParam;
        private Vector3 oldPosition;
        private bool isAllowToDisposeDrag;

        public WorldCameraController(
            PlayerCamera playerCamera, 
            PointerService pointerService, 
            SmoothTransitionParam smoothTransitionParam)
        {
            cameraTransform = playerCamera.transform;
            this.pointerService = pointerService;
            startPosition = cameraTransform.position;
            this.smoothTransitionParam = smoothTransitionParam;
        }

        public void Enable()
        {
            if (isOn)
                return;

            isOn = true;

            pointerService.OnDrag
                .Subscribe(delta => deltaPositon = delta).AddTo(disposables);

            pointerService.OnPointerDown
                .Subscribe(_ =>
                {
                    dragDisposable?.Dispose();
                    velocity = Vector3.zero;
                    isAllowToDisposeDrag = false;
                    dragDisposable = Observable.EveryUpdate()
                        .Subscribe(Update)
                        .AddTo(disposables);
                }).AddTo(disposables);

            pointerService.OnPointerUp
                .Subscribe(_ =>
                {
                    deltaPositon = Vector3.zero;
                    isAllowToDisposeDrag = true;
                }).AddTo(disposables);
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

        private void Update(long _)
        {
            Vector3 newPosition = Vector3.SmoothDamp(
                cameraTransform.position,
                cameraTransform.position
                    - new Vector3(deltaPositon.x, 0f, deltaPositon.y)
                    * smoothTransitionParam.MoveSpeed, ref velocity, smoothTransitionParam.SmoothTime);

            if (isAllowToDisposeDrag && newPosition.IsClose(oldPosition, float.Epsilon))
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
    }
}
