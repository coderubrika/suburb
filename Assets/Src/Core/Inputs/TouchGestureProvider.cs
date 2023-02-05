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
        private GestureEventData lastData;

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
                SetupTouch(touchId);
        }

        private void SetupTouch(int touchId)
        {
            bool isTouched = (int)Touchscreen.current.touches[touchId].press.ReadValue() == 1;

            if (isTouched)
            {
                if (touchStates[touchId] == GestureType.None)
                {
                    touchStates[touchId] = GestureType.Down;
                    lastData = GetEventData(touchId, GestureType.Down);
                    OnPointerDown.Execute(lastData);
                    return;
                }

                Vector2 delta = Touchscreen.current.touches[touchId].delta.ReadValue();

                if (touchStates[touchId] == GestureType.Down && !delta.IsClose(dragTreshold))
                {
                    isDragging = true;
                    touchStates[touchId] = GestureType.DragStart;
                    lastData = GetEventData(touchId, GestureType.DragStart);
                    OnDragStart.Execute(lastData);
                    return;
                }

                if (touchStates[touchId] == GestureType.DragStart)
                {
                    touchStates[touchId] = GestureType.Drag;
                    lastData = GetEventData(touchId, GestureType.Drag);
                    OnDrag.Execute(lastData);
                    return;
                }

                if (touchStates[touchId] == GestureType.Drag)
                {
                    lastData = GetEventData(touchId, GestureType.Drag);
                    OnDrag.Execute(lastData);
                    return;
                }
            }
            else if (touchStates[touchId] != GestureType.None)
            {
                touchStates[touchId] = GestureType.Up;
                lastData = lastData.CopyWithType(GestureType.Up);
                OnPointerUp.Execute(lastData);

                if (isDragging)
                {
                    touchStates[touchId] = GestureType.DragEnd;
                    isDragging = false;
                    lastData = lastData.CopyWithType(GestureType.DragEnd);
                    OnDragEnd.Execute(lastData);
                }

                touchStates[touchId] = GestureType.None;
                return;
            }
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
