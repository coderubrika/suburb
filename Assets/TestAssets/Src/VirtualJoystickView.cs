using DG.Tweening;
using UniRx;
using UnityEngine;

namespace TestAssets.Src
{
    public class VirtualJoystickView : MonoBehaviour
    {
        [SerializeField] private RectTransform joystickField;
        [SerializeField] private RectTransform joystickHandler;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private readonly CompositeDisposable compositeDisposable = new();
        
        private float radius;

        public float Radius 
        { 
            get
            {
                radius = radius == 0 ? joystickField.sizeDelta.x / 2 : radius;
                return radius;
            }
        }
        
        public void Connect(VirtualJoystickController virtualJoystickController)
        {
            virtualJoystickController.OnDown
                .Subscribe(position =>
                {
                    joystickHandler.anchoredPosition = Vector2.zero;
                    DOTween.Kill(canvasGroup);
                    joystickField.position = position;
                    canvasGroup.DOFade(1, 0.4f);
                })
                .AddTo(compositeDisposable);

            virtualJoystickController.OnDirectionAndForce
                .Subscribe(data =>
                {
                    joystickHandler.anchoredPosition = data.Direction * Radius;
                })
                .AddTo(compositeDisposable);
            
            virtualJoystickController.OnUp
                .Subscribe(_ =>
                {
                    DOTween.Kill(canvasGroup);
                    canvasGroup.DOFade(0, 0.4f);
                })
                .AddTo(compositeDisposable);
        }

        public void Disconnect()
        {
            compositeDisposable.Clear();
        }
    }
}