using Suburb.Common;
using Suburb.Core;
using Suburb.Interactables;
using System;
using UniRx;
using UnityEngine;

namespace Suburb.Selectors
{
    public class InteractablesSelector
    {
        private readonly Camera playerCamera;
        private readonly PointerService pointerService;
        private readonly InteractionRepository interactionRepository;

        private readonly CompositeDisposable disposables = new();

        private bool isOn;
        private bool isDragging;
        public InteractablesSelector(
            PlayerCamera playerCamera,
            PointerService pointerService,
            InteractionRepository interactionRepository
            )
        {
            this.playerCamera = playerCamera.GetCamera();
            this.pointerService = pointerService;
            this.interactionRepository = interactionRepository;
        }

        public void SetEnableChecks(bool isOn)
        {
            if (this.isOn == isOn)
                return;

            this.isOn = isOn;

            if (this.isOn)
            {
                pointerService.OnPointerDown
                    .Subscribe(_ =>
                    {
                        isDragging = false;

                        IDisposable dragDisposable = null;
                        dragDisposable = pointerService.OnDrag
                            .Subscribe(_ =>
                            {
                                dragDisposable?.Dispose();
                                isDragging = true;
                            })
                            .AddTo(disposables);
                    })
                    .AddTo(disposables);

                

                pointerService.OnPointerUp
                    .Subscribe(_ =>
                    {
                        if (!isDragging)
                            CheckPoint(pointerService.PointerPositionOnScreen.Value);
                    })
                    .AddTo(disposables);
            }
            else
            {
                disposables.Clear();
            }
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
