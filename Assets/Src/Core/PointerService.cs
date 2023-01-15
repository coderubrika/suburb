using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Suburb.Core
{
    public class PointerService: IInitializable, IDisposable, ITickable
    {
        private readonly PointerControls pointerInput;

        public readonly ReactiveProperty<Vector2> PointerPositionOnScreen = new();
        public readonly ReactiveCommand OnPointerDown = new();

        public PointerService()
        {
            pointerInput = new PointerControls();

            pointerInput.Map.Down.performed += context => OnPointerDown.Execute();
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
            PointerPositionOnScreen.Value = pointerInput.Map.PositionOnScreen.ReadValue<Vector2>();
        }
    }
}
