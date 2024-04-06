using Suburb.Cameras;
using Suburb.Common;
using Suburb.Inputs;
using Suburb.Interactables;
using UniRx;
using UnityEngine;

namespace Suburb.Detectors
{
    public class PickDetector
    {
        private readonly IGestureProvider gestureProvider;
        
        private Camera camera;

        private readonly CompositeDisposable disposables = new();

        public ReactiveCommand<PickEventData> OnPick { get; } = new();

        public PickDetector(IGestureProvider gestureProvider)
        {
            this.gestureProvider = gestureProvider;
        }

        public void Enable(Camera camera)
        {
            this.camera = camera;
            disposables.Clear();

            // gestureProvider.OnPointerUp
            //     .Where(data => !gestureProvider.IsDragging(data.Id))
            //     .Subscribe(data => CheckPoint(data.Position))
            //     .AddTo(disposables);
        }

        public void Disable()
        {
            disposables.Clear();
        }

        private void CheckPoint(Vector2 point)
        {
            Ray ray = camera.ScreenPointToRay(point);

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
