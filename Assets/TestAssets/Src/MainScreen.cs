using Suburb.Inputs;
using UniRx;
using UnityEngine;
using Zenject;

namespace TestAssets.Src
{
    public class MainScreen : MonoBehaviour
    {
        private PlayerController playerController;
        private PointerGestureProvider gestureProvider;
        private Camera camera;
        
        [SerializeField] private VirtualJoystick joystick;
        [SerializeField] private RectTransform joystickArea;
        
        [SerializeField] private VirtualJoystick joystick1;
        [SerializeField] private RectTransform joystickArea1;
        
        private readonly CompositeDisposable disposables = new();
        
        
        [Inject]
        private void Construct(PointerGestureProvider gestureProvider, PlayerController playerController, Camera camera)
        {
            this.gestureProvider = gestureProvider;
            this.playerController = playerController;
            this.camera = camera;
            this.gestureProvider = gestureProvider;
        }
        
        private void OnEnable()
        {
            SetupSession(new SwipeGestureSession(joystickArea, null), joystick);
            SetupSession(new SwipeGestureSession(joystickArea1, null), joystick1);
        }

        private void SetupSession(SwipeGestureSession session, VirtualJoystick virtualJoystick)
        {
            gestureProvider.AddSession(session)
                .AddTo(disposables);
            
            virtualJoystick.Connect(session);

            session.OnDown
                .Subscribe(_ => playerController.StartMoving())
                .AddTo(disposables);
            
            session.OnUp
                .Subscribe(_ => playerController.StopMoving())
                .AddTo(disposables);
            
            virtualJoystick.OnDirectionAndForce
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
            joystick.Disconnect();
            disposables.Clear();
        }
    }
}