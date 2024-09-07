using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace TestAssets.Src
{
    public class VirtualJoystickController
    {
        private readonly DragZoomGestureSession gestureSession;
        private readonly CompositeDisposable disposables = new();
        
        private Vector2 startPosition;
        private Vector2 currentDelta;
        private float radius;
        
        public ReactiveCommand<Vector2> OnDown { get; } = new();
        public ReactiveCommand OnUp { get; } = new();
        public ReactiveProperty<(Vector2 Direction, float Force)> OnDirectionAndForce { get; } = new();
        
        public VirtualJoystickController(DragZoomGestureSession gestureSession, float radius)
        {
            this.gestureSession = gestureSession;
            this.radius = radius;
        }
        
        public void Enable()
        {
            gestureSession.OnDown
                .Subscribe(position =>
                {
                    startPosition = position;
                    currentDelta = Vector2.zero;
                    OnDown.Execute(startPosition);
                })
                .AddTo(disposables);

            gestureSession.OnDrag
                .Subscribe(delta =>
                {
                    Vector2 rectDelta = gestureSession.Bounds.TransformVector(delta);
                    currentDelta += rectDelta;
                    float magnitude = currentDelta.magnitude;
                    float force = magnitude / radius;
                    force = Mathf.Clamp(force, 0, 1);
                    Vector2 normalized = currentDelta / magnitude;
                    Vector2 direction = normalized * force;
                    OnDirectionAndForce.Value = (direction, force);
                })
                .AddTo(disposables);
            
            gestureSession.OnUp
                .Subscribe(_ =>
                {
                    OnDirectionAndForce.Value = (Vector2.zero, 0);
                    OnUp.Execute();
                })
                .AddTo(disposables);
        }

        public void Disable()
        {
            disposables.Clear();
        }
    }
}