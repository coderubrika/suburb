using Suburb.Utils;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using Zenject;

namespace Suburb.Core.Inputs
{
    public class TouchGestureProvider : IGestureProvider, ITickable, IInitializable, IDisposable
    {
        private readonly GestureType[] touchStates = Enumerable.Repeat(GestureType.None, Touchscreen.current.touches.Count).ToArray();
        // TODO move to project settings
        private readonly float dragTreshold = 5;
        private readonly float zoomFactor = 0.01f;
        private bool[] isDraggings = new bool[Touchscreen.current.touches.Count];
        private bool isEnabled;
        private bool isDoubleTouchDragging;
        private Vector2 middlePoint;
        private float doubleTouchDistance;

        public ReactiveCommand<GestureEventData> OnPointerDown { get; } = new();
        public ReactiveCommand<GestureEventData> OnPointerUp { get; } = new();
        public ReactiveCommand<GestureEventData> OnDragStart { get; } = new();
        public ReactiveCommand<GestureEventData> OnDrag { get; } = new();
        public ReactiveCommand<GestureEventData> OnDragEnd { get; } = new();
        public ReactiveCommand<GestureEventData> OnZoom { get; } = new();
        public ReactiveCommand<GestureEventData> OnDragWithDoubleTouch { get; } = new();

        public bool IsDragging(int touchId)
        {
            return isDraggings[touchId];
        }

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

            for(int touchId = 0; touchId < touchStates.Length; touchId++)
                SetupTouch(touchId);

            SetupDoubleTouch();
        }

        private void SetupDoubleTouch()
        {
            bool isAllowedDoubleTouch = touchStates[0] == GestureType.Drag && touchStates[1] == GestureType.Drag;

            TouchControl touch0 = Touchscreen.current.touches[0];
            TouchControl touch1 = Touchscreen.current.touches[1];

            Vector2 position0 = touch0.position.ReadValue();
            Vector2 position1 = touch1.position.ReadValue();

            Vector2 newMiddlePoint = (position0 + position1) / 2;
            float newDoubleTouchDistance = (position1 - position0).magnitude;

            if (!isAllowedDoubleTouch)
                return;

            if (!isDoubleTouchDragging)
            {
                Vector2 delta0 = touch0.delta.ReadValue();
                Vector2 delta1 = touch1.delta.ReadValue();

                Vector2 touch0Position = position0 - delta0;
                Vector2 touch1Position = position1 - delta1;

                middlePoint = (touch0Position + touch1Position) / 2;
                doubleTouchDistance = (touch1Position - touch0Position).magnitude;
            }

            Vector2 moveDelta = newMiddlePoint - middlePoint;

            float resultZoom = (newDoubleTouchDistance - doubleTouchDistance) * zoomFactor;

            middlePoint = newMiddlePoint;
            doubleTouchDistance = newDoubleTouchDistance;

            OnZoom.Execute(new GestureEventData()
            {
                Id = 0,
                Delta = moveDelta,
                Position = middlePoint,
                ZoomDelta = new Vector2(0, resultZoom),
                Type = GestureType.Zoom
            });

            OnDragWithDoubleTouch.Execute(new GestureEventData()
            {
                Id = 0,
                Delta = moveDelta,
                Position = middlePoint,
                ZoomDelta = new Vector2(0, resultZoom),
                Type = GestureType.Drag
            });
        }

        private void SetupTouch(int touchId)
        {
            bool isTouched = (int)Touchscreen.current.touches[touchId].press.ReadValue() == 1;

            if (isTouched)
            {
                if (touchStates[touchId] == GestureType.None)
                {
                    touchStates[touchId] = GestureType.Down;
                    SendPointerDown(touchId);
                    return;
                }

                Vector2 delta = Touchscreen.current.touches[touchId].delta.ReadValue();

                if (touchStates[touchId] == GestureType.Down && !delta.IsClose(dragTreshold))
                {
                    touchStates[touchId] = GestureType.DragStart;
                    SendDragStart(touchId);
                    return;
                }

                if (touchStates[touchId] == GestureType.DragStart)
                {
                    touchStates[touchId] = GestureType.Drag;
                    SendDrag(touchId);
                    return;
                }

                if (touchStates[touchId] == GestureType.Drag)
                {
                    SendDrag(touchId);
                    return;
                }
            }
            else if (touchStates[touchId] != GestureType.None)
            {
                touchStates[touchId] = GestureType.Up;
                SendPointerUp(touchId);

                if (isDraggings[touchId])
                {
                    touchStates[touchId] = GestureType.DragEnd;
                    SendDragEnd(touchId);
                }

                touchStates[touchId] = GestureType.None;
            }
        }

        private int GetPressedCount()
        {
            ReadOnlyArray<TouchControl> touches = Touchscreen.current.touches;

            int pressedSum = 0;

            for (int touchId = 0; touchId < touches.Count; touchId++)
            {
                bool isPressed = (int)Touchscreen.current.touches[touchId].press.ReadValue() == 1;

                if (isPressed)
                    pressedSum += 1;
                else
                    break;
            }

            return pressedSum;
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

        private void SendPointerDown(int touchId)
        {
            OnPointerDown.Execute(GetEventData(touchId, GestureType.Down));
        }

        private void SendPointerUp(int touchId)
        {
            OnPointerUp.Execute(GetEventData(touchId, GestureType.Up));
        }

        private void SendDragStart(int touchId)
        {
            isDraggings[touchId] = true;
            OnDragStart.Execute(GetEventData(touchId, GestureType.DragStart));
        }

        private void SendDrag(int touchId)
        {
            OnDrag.Execute(GetEventData(touchId, GestureType.Drag));
        }

        private void SendDragEnd(int touchId)
        {
            isDraggings[touchId] = false;
            OnDragEnd.Execute(GetEventData(touchId, GestureType.DragEnd));
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
