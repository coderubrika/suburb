using System;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace TestAssets.Src
{
    public class MainScreen : MonoBehaviour
    {
        private PlayerController playerController;
        private DragZoomGestureProvider gestureProvider;
        private Camera camera;
        
        [SerializeField] private VirtualJoystickView joystickView;
        
        private readonly CompositeDisposable disposables = new();
        
        private DragZoomGestureSession session;
        private VirtualJoystickController joystickController;
        
        [Inject]
        private void Construct(DragZoomGestureProvider gestureProvider, PlayerController playerController, Camera camera)
        {
            this.gestureProvider = gestureProvider;
            this.playerController = playerController;
            this.camera = camera;
            this.gestureProvider = gestureProvider;
            
            session = new DragZoomGestureSession(transform as RectTransform, null);
            joystickController = new VirtualJoystickController(session, joystickView.Radius);
        }
        
        private void OnEnable()
        {
            gestureProvider.Enable(session);
            joystickController.Enable();
            joystickView.Connect(joystickController);

            joystickController.OnDown
                .Subscribe(_ => playerController.StartMoving())
                .AddTo(disposables);
            
            joystickController.OnUp
                .Subscribe(_ => playerController.StopMoving())
                .AddTo(disposables);
            
            joystickController.OnDirectionAndForce
                .ObserveOnMainThread()
                .Subscribe(data =>
                {
                    Vector3 cameraForward = new Vector3(camera.transform.forward.x, 0f, camera.transform.forward.z).normalized;
                    Vector3 cameraRight = new Vector3(camera.transform.right.x, 0f, camera.transform.right.z).normalized;
                    Vector3 moveDirection = (cameraForward * data.Direction.y + cameraRight * data.Direction.x).normalized;
                    playerController.PutDirection(moveDirection * data.Force);
                })
                .AddTo(disposables);
        }

        private void OnDisable()
        {
            joystickView.Disconnect();
            joystickController.Disable();
            gestureProvider.Disable(session);
            disposables.Clear();
        }
    }
}