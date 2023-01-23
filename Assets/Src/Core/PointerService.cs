using System;
using UniRx;
using UnityEngine;
using Zenject;
using static UnityEngine.InputSystem.InputAction;

namespace Suburb.Core
{
    public class PointerService: IInitializable, IDisposable, ITickable
    {
        private readonly PointerControls pointerInput;

        private bool isDradding;
        private Vector2 oldPosition;

        public ReactiveProperty<Vector2> PointerPositionOnScreen { get; } = new();
        public ReactiveCommand OnPointerDown { get; } = new();
        public ReactiveCommand OnPointerUp { get; } = new();
        public ReactiveCommand<Vector2> OnDrag { get; } = new();

#if UNITY_STANDALONE
        public ReactiveCommand<float> OnMouseScroll { get; } = new();
#endif
        public PointerService()
        {
            pointerInput = new PointerControls();

            pointerInput.All.Down.performed += PointerDown;
            pointerInput.All.Up.performed += PointerUp;

#if UNITY_STANDALONE
            pointerInput.Mouse.Scroll.performed += context => OnMouseScroll.Execute(context.ReadValue<Vector2>().y / 360f);
#endif
        }

        public void Dispose()
        {
            pointerInput.Disable();
        }

        public void Initialize()
        {
            pointerInput.Enable();
        }

        public void Tick()
        {
            PointerPositionOnScreen.Value = pointerInput.All.PositionOnScreen.ReadValue<Vector2>();

            if (isDradding)
            {
                Vector2 newPosition = PointerPositionOnScreen.Value;
                OnDrag.Execute(newPosition - oldPosition);
            }

            oldPosition = PointerPositionOnScreen.Value;
        }

        private void PointerDown(CallbackContext context)
        {
            OnPointerDown.Execute();

            isDradding = true;
        }

        private void PointerUp(CallbackContext context)
        {
            isDradding = false;

            OnPointerUp.Execute();
        }
    }
}
