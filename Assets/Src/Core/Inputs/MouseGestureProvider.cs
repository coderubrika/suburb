using System;
using UniRx;
using UnityEngine;
using Zenject;
using static UnityEngine.InputSystem.InputAction;
using Suburb.Utils;

namespace Suburb.Core.Inputs
{
    public class MouseGestureProvider : IGestureProvider, IInitializable, IDisposable, ITickable
    {
        private readonly MouseControls inputControls;

        // TODO move to project settings
        private readonly float dragTreshold = 5;

        private bool isDragging;

        private GestureType currentGesture = GestureType.None;

        public ReactiveCommand<GestureEventData> OnPointerDown { get; } = new();

        public ReactiveCommand<GestureEventData> OnPointerUp { get; } = new();

        public ReactiveCommand<GestureEventData> OnDragStart { get; } = new();

        public ReactiveCommand<GestureEventData> OnDrag { get; } = new();

        public ReactiveCommand<GestureEventData> OnDragEnd { get; } = new();

        public ReactiveCommand<GestureEventData> OnZoom { get; } = new();

        public bool IsDragging => isDragging;

        public MouseGestureProvider()
        {
            inputControls = new MouseControls();

            inputControls.Mouse.Down.performed += PointerDown;
            inputControls.Mouse.Up.performed += PointerUp;
            inputControls.Mouse.Zoom.performed += Zoom;
        }

        public void Dispose()
        {
            inputControls.Disable();
        }

        public void Initialize()
        {
            inputControls.Enable();
        }

        public void Tick()
        {
            if (currentGesture == GestureType.None)
                return;

            if (currentGesture == GestureType.Down
                && !inputControls.Mouse.Delta.ReadValue<Vector2>().IsClose(dragTreshold))
            {
                isDragging = true;
                currentGesture = GestureType.DragStart;
                OnDragStart.Execute(GetEventData(GestureType.DragStart));
                return;
            }

            if (currentGesture == GestureType.DragStart)
            {
                currentGesture = GestureType.Drag;
                OnDrag.Execute(GetEventData(GestureType.Drag));
                return;
            }

            if (currentGesture == GestureType.Drag)
            {
                OnDrag.Execute(GetEventData(GestureType.Drag));
                return;
            }

            if (currentGesture == GestureType.Up && isDragging)
            {
                currentGesture = GestureType.DragEnd;
                isDragging = false;
                OnDragEnd.Execute(GetEventData(GestureType.DragEnd));
                currentGesture = GestureType.None;
                return;
            }
        }

        private void PointerDown(CallbackContext context)
        {
            currentGesture = GestureType.Down;
            OnPointerDown.Execute(GetEventData(GestureType.Down));
        }

        private void PointerUp(CallbackContext context)
        {
            currentGesture = GestureType.Up;
            OnPointerUp.Execute(GetEventData(GestureType.Up));
        }

        private void Zoom(CallbackContext context)
        {
            OnZoom.Execute(GetEventData(GestureType.Zoom));
        }

        private GestureEventData GetEventData (GestureType gestureType)
        {
            return new GestureEventData()
            {
                Id = inputControls.Mouse.Id.ReadValue<int>(),
                Position = inputControls.Mouse.Position.ReadValue<Vector2>(),
                Delta = inputControls.Mouse.Delta.ReadValue<Vector2>(),
                ZoomDelta = inputControls.Mouse.Zoom.ReadValue<Vector2>(),
                Type = gestureType
            };
        }
    }
}