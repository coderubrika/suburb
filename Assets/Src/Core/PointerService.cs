using Suburb.Utils;
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
        private readonly float dragTreshold = 5;

        private bool isDradding;
        private bool isDraddingCandidate;
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

            if (!isDraddingCandidate && !isDradding)
                return;

            if (IsWhongTouchPosition(PointerPositionOnScreen.Value))
                return;

            if (isDraddingCandidate && !oldPosition.IsClose(PointerPositionOnScreen.Value, dragTreshold))
            {
                Debug.Log($"Check dragging");
                isDradding = true;
                isDraddingCandidate = false;
            }

            if (isDradding)
            {
                //Debug.Log("Dragging");
                OnDrag.Execute(PointerPositionOnScreen.Value - oldPosition);
                oldPosition = PointerPositionOnScreen.Value;
            }
        }

        private void PointerDown(CallbackContext context)
        {
            Debug.Log("Pointer Down");
            OnPointerDown.Execute();

            isDraddingCandidate = true;
            oldPosition = PointerPositionOnScreen.Value;
        }

        private void PointerUp(CallbackContext context)
        {
            isDraddingCandidate = false;
            isDradding = false;
            
            //Debug.Log("Pointer Up");
            OnPointerUp.Execute();
        }

        private static bool IsWhongTouchPosition(Vector2 source)
        {
            return source.x < 0 || source.y < 0 || (source.x == 0 && source.y == 0);
        }
    }
}
