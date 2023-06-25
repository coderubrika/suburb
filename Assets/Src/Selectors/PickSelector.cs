using Suburb.Common;
using Suburb.Core;
using Suburb.Core.Inputs;
using Suburb.Interactables;
using Suburb.Utils;
using System;
using UniRx;
using UnityEngine;

namespace Suburb.Selectors
{
    public class PickSelector
    {
        private readonly Camera playerCamera;
        private readonly IGestureProvider gestureProvider;
        private readonly InteractablesStore interactionRepository;

        private readonly CompositeDisposable disposables = new();

        private bool isOn;
        public PickSelector(
            PlayerCamera playerCamera,
            IGestureProvider gestureProvider,
            InteractablesStore interactionRepository)
        {
            this.playerCamera = playerCamera.GetCamera();
            this.gestureProvider = gestureProvider;
            this.interactionRepository = interactionRepository;
        }

        public void SetEnableChecks(bool isOn)
        {
            if (this.isOn == isOn)
                return;

            this.isOn = isOn;

            if (this.isOn)
            {
                gestureProvider.OnPointerUp
                    .Where(data => !gestureProvider.IsDragging(data.Id))
                    .Subscribe(data => CheckPoint(data.Position))
                    .AddTo(disposables);
            }
            else
                disposables.Clear();
        }

        private void CheckPoint(Vector2 point)
        {
            Ray ray = playerCamera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (interactionRepository.CheckGameObject(hit.transform.gameObject, out IInteractable interactable))
                {
                    interactable.Interact(new BaseInteractEventData { Ray = ray, Distance = hit.distance });
                }
            }
        }
    }
}
