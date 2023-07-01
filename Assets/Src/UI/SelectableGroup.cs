using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Suburb.UI
{
    public class SelectableGroup : MonoBehaviour
    {
        [SerializeField] private Selectable mainSelectable;
        [SerializeField] private Selectable[] dependentSelectables;

        private bool isInteractable;

        private void Awake()
        {
            SetInteractables();

            mainSelectable.OnPointerDownAsObservable()
                .Subscribe(eventData =>
                {
                    foreach (var selectable in dependentSelectables)
                        selectable.OnPointerDown(eventData);
                })
                .AddTo(this);

            mainSelectable.OnPointerUpAsObservable()
                .Subscribe(eventData =>
                {
                    foreach (var selectable in dependentSelectables)
                        selectable.OnPointerUp(eventData);
                })
                .AddTo(this);

            mainSelectable.OnPointerExitAsObservable()
                .Subscribe(eventData =>
                {
                    foreach (var selectable in dependentSelectables)
                        selectable.OnPointerExit(eventData);
                })
                .AddTo(this);

            mainSelectable.OnPointerEnterAsObservable()
                .Subscribe(eventData =>
                {
                    foreach (var selectable in dependentSelectables)
                        selectable.OnPointerEnter(eventData);
                })
                .AddTo(this);

            mainSelectable.OnSelectAsObservable()
                .Subscribe(eventData =>
                {
                    foreach (var selectable in dependentSelectables)
                        selectable.OnSelect(eventData);
                })
                .AddTo(this);

            mainSelectable.OnDeselectAsObservable()
                .Subscribe(eventData =>
                {
                    foreach (var selectable in dependentSelectables)
                        selectable.OnDeselect(eventData);
                })
                .AddTo(this);
        }

        private void Update()
        {
            if (isInteractable != mainSelectable.interactable)
                SetInteractables();
        }

        private void SetInteractables()
        {
            isInteractable = mainSelectable.interactable;
            foreach (var selectable in dependentSelectables)
                selectable.interactable = isInteractable;
        }
    }
}