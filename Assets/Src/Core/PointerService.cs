using Suburb.Utils;
using System;
using UniRx;
using UnityEngine;
using Zenject;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
namespace Suburb.Core
{
    public class PointerService
    {
        /*private readonly PointerControls pointerInput;
        private readonly float dragTreshold = 5;

        private bool isDradding;
        private bool isDraddingCandidate;
        private Vector2 oldPosition;

        public ReactiveProperty<Vector2> PointerPositionOnScreen { get; } = new();
        public ReactiveCommand OnPointerDown { get; } = new();
        public ReactiveCommand OnPointerUp { get; } = new();
        public ReactiveCommand<Vector2> OnDrag { get; } = new();

#if UNITY_STANDALONE
        public ReactiveCommand<float> OnMouseScroll { get; } = new();
#endif
        public PointerService()
        {
            pointerInput = new PointerControls();

            pointerInput.All.Down.performed += PointerDown;
            pointerInput.All.Up.performed += PointerUp;

#if UNITY_STANDALONE
            pointerInput.Mouse.Scroll.performed += context => OnMouseScroll.Execute(context.ReadValue<Vector2>().y / 360f);
#endif
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
            PointerPositionOnScreen.Value = pointerInput.All.PositionOnScreen.ReadValue<Vector2>();

            if (!isDraddingCandidate && !isDradding)
                return;

            if (IsWhongTouchPosition(PointerPositionOnScreen.Value))
                return;

            if (isDraddingCandidate && !oldPosition.IsClose(PointerPositionOnScreen.Value, dragTreshold))
            {
                Debug.Log($"Check dragging");
                isDradding = true;
                isDraddingCandidate = false;
            }

            if (isDradding)
            {
                //Debug.Log("Dragging");
                OnDrag.Execute(PointerPositionOnScreen.Value - oldPosition);
                oldPosition = PointerPositionOnScreen.Value;
            }
        }

        private void PointerDown(CallbackContext context)
        {
            Debug.Log("Pointer Down");
            OnPointerDown.Execute();

            isDraddingCandidate = true;
            oldPosition = PointerPositionOnScreen.Value;
        }

        private void PointerUp(CallbackContext context)
        {
            isDraddingCandidate = false;
            isDradding = false;
            
            Debug.Log("Pointer Up");
            OnPointerUp.Execute();
        }

        private static bool IsWhongTouchPosition(Vector2 source)
        {
            return source.x < 0 || source.y < 0 || (source.x == 0 && source.y == 0);
        }*/

        /*private void Update()
        {
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
