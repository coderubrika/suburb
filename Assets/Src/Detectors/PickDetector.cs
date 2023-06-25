using Suburb.Common;
using Suburb.Core.Inputs;
using Suburb.Interactables;
using UniRx;
using UnityEngine;

namespace Suburb.Detectors
{
    public class PickDetector
    {
        private readonly Camera playerCamera;
        private readonly IGestureProvider gestureProvider;

        private readonly CompositeDisposable disposables = new();

        public ReactiveCommand<PickEventData> OnPick { get; } = new();

        public PickDetector(
            PlayerCamera playerCamera,
            IGestureProvider gestureProvider)
        {
            this.playerCamera = playerCamera.GetCamera();
            this.gestureProvider = gestureProvider;
        }

        public void Enable()
        {
            disposables.Clear();

            gestureProvider.OnPointerUp
                .Where(data => !gestureProvider.IsDragging(data.Id))
                .Subscribe(data => CheckPoint(data.Position))
                .AddTo(disposables);
        }

        public void Disable()
        {
            disposables.Clear();
        }

        private void CheckPoint(Vector2 point)
        {
            Ray ray = playerCamera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out RaycastHit hit))
                OnPick.Execute(new PickEventData
                {
                    GameObject = hit.transform.gameObject,
                    Ray = ray,
                    Distance = hit.distance
                });
        }
    }
}
