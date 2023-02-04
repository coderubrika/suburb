using Suburb.Utils;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Suburb.Core.Inputs
{
    public class TouchGestureProvider : IGestureProvider, ITickable, IInitializable, IDisposable
    {
        private readonly GestureType[] touchStates = Enumerable.Repeat(GestureType.None, 1).ToArray();
        // TODO move to project settings
        private readonly float dragTreshold = 5;

        private bool isDragging;
        private bool isEnabled;

        public ReactiveCommand<GestureEventData> OnPointerDown { get; } = new();

        public ReactiveCommand<GestureEventData> OnPointerUp { get; } = new();

        public ReactiveCommand<GestureEventData> OnDragStart { get; } = new();

        public ReactiveCommand<GestureEventData> OnDrag { get; } = new();

        public ReactiveCommand<GestureEventData> OnDragEnd { get; } = new();

        public ReactiveCommand<GestureEventData> OnZoom { get; } = new();

        public bool IsDragging => isDragging;

        public void Dispose()
        {
            isEnabled = false;
        }

        public void Initialize()
        {
            isEnabled = true;
        }

        public void Tick()
        {
            if (!isEnabled)
                return;

            for (int touchId = 0; touchId < touchStates.Length; touchId++)
            {
                SetupTouched(touchId);

                if (IsTouchedDragStart(touchId))
                    continue;

                if (IsTouchedDrag(touchId))
                    continue;

                if (touchStates[touchId] == GestureType.Drag)
                {
                    OnDrag.Execute(GetEventData(touchId, GestureType.Drag));
                    continue;
                }

                if (touchStates[touchId] == GestureType.Up && isDragging)
                {
                    touchStates[touchId] = GestureType.DragEnd;
                    isDragging = false;
                    OnDragEnd.Execute(GetEventData(touchId, GestureType.DragEnd));
                    touchStates[touchId] = GestureType.None;
                    continue;
                }
            }
        }

        private void SetupTouched(int touchId)
        {
            bool isTouched = (int)Touchscreen.current.touches[touchId].press.ReadValue() == 1;

            if (isTouched && touchStates[touchId] == GestureType.None)
            {
                touchStates[touchId] = GestureType.Down;
                OnPointerDown.Execute(GetEventData(touchId, GestureType.Down));
            }

            if (!isTouched && touchStates[touchId] != GestureType.None && touchStates[touchId] != GestureType.Up)
            {
                touchStates[touchId] = GestureType.Up;
                OnPointerDown.Execute(GetEventData(touchId, GestureType.Up));
            }
        }

        private bool IsTouchedDragStart(int touchId)
        {
            Vector2 delta = Touchscreen.current.touches[touchId].delta.ReadValue();
            
            if (touchStates[touchId] == GestureType.Down && !delta.IsClose(dragTreshold))
            {
                isDragging = true;
                touchStates[touchId] = GestureType.DragStart;
                OnDragStart.Execute(GetEventData(touchId, GestureType.DragStart));

                return true;
            }

            return false;
        }

        private bool IsTouchedDrag(int touchId)
        {
            if (touchStates[touchId] == GestureType.DragStart)
            {
                touchStates[touchId] = GestureType.Drag;
                OnDrag.Execute(GetEventData(touchId, GestureType.Drag));
                return true;
            }

            return false;
        }

        private GestureEventData GetEventData(int touchId, GestureType gestureType)
        {
            return new GestureEventData()
            {
                Id = touchId,
                Position = Touchscreen.current.touches[touchId].position.ReadValue(),
                Delta = Touchscreen.current.touches[touchId].delta.ReadValue(),
                ZoomDelta = Vector2.zero,
                Type = gestureType
            };
        }

       /* private void Update()
        {
            // для начала надо определить начало нажатия и конец нажатия
            if ((!(endScaleTween is { active: true }) || !endScaleTween.IsPlaying())
                && !photoCarouselNew.IsDragging
                && (int)(Touchscreen.current.touches[0].press.ReadValue() + Touchscreen.current.touches[1].press.ReadValue()) == 2)
            {
                var touch0 = Touchscreen.current.touches[0];
                var touch1 = Touchscreen.current.touches[1];
                if (!zoom)
                {
                    initialTouch0Position = touch0.position.ReadValue();
                    initialTouch1Position = touch1.position.ReadValue();
                    zoom = true;
                    photoCarouselNew.SetScrollEnabled(false);
                    var itemTransform = photoCarouselNew.CurrentItem.transform as RectTransform;
                    initialLocalPosition = itemTransform.localPosition;
                    additionalPosition = itemTransform.position - (Vector3)((initialTouch0Position + initialTouch1Position) / 2);
                }
                else
                {
                    var touch0Position = touch0.position.ReadValue();
                    var touch1Position = touch1.position.ReadValue();
                    var scaleFactor = GetScaleFactor(touch0Position,
                        touch1Position,
                        initialTouch0Position,
                        initialTouch1Position);
                    Vector3 currentMidPoint = (touch0Position + touch1Position) / 2;
                    var itemTransform = (photoCarouselNew.CurrentItem.transform as RectTransform);
                    itemTransform.position = currentMidPoint + additionalPosition * scaleFactor;
                    itemTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                    darkBg.alpha = (scaleFactor - 1f) * .8f;
                }
            }
            else if (zoom)
            {
                zoom = false;
                var itemTransform = (photoCarouselNew.CurrentItem.transform as RectTransform);
                endScaleTween = DOTween.Sequence()
                    .Join(itemTransform.DOLocalMove(initialLocalPosition, .2f))
                    .Join(itemTransform.DOScale(Vector3.one, .2f))
                    .Join(darkBg.DOFade(0f, .2f))
                    .OnComplete(() =>
                    {
                        photoCarouselNew.SetScrollEnabled(true);
                        var selectedIndex = photoCarouselNew.SelectedChild.Value;
                        photoCarouselNew.GoToElement(selectedIndex, false);
                    });
            }
        }*/
    }
}
