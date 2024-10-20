using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace TestAssets.Src
{
    public class MainScreen : MonoBehaviour
    {
        private PlayerController playerController;
        private PointerGestureConnector gestureConnector;
        private Camera camera;
        private KeyboardInputProvider keyboardInputProvider;
        
        [SerializeField] private Stick joystick;
        [SerializeField] private RectTransform joystickArea;
        
        [SerializeField] private Stick joystick1;
        [SerializeField] private RectTransform joystickArea1;
        
        [SerializeField] private Stick joystick2;
        [SerializeField] private RectTransform joystickArea2;
        
        private readonly CompositeDisposable disposables = new();
        private Vector2 moveDirectionFromKeyboard;
        
        [Inject]
        private void Construct(
            PointerGestureConnector gestureConnector, 
            PlayerController playerController, 
            Camera camera,
            KeyboardInputProvider keyboardInputProvider)
        {
            this.gestureConnector = gestureConnector;
            this.playerController = playerController;
            this.camera = camera;
            this.gestureConnector = gestureConnector;
            this.keyboardInputProvider = keyboardInputProvider;
        }
        
        private void OnEnable()
        {
            SetupSession(new SwipeGestureSession(joystickArea, null), joystick);
            SetupSession(new SwipeGestureSession(joystickArea1, null), joystick1);
            SetupSession(new SwipeGestureSession(joystickArea2, null), joystick2);

            KeyboardSession keyboardSession = keyboardInputProvider.CreateSession()
                .AddTo(disposables);
            
            keyboardSession.OnKey(Key.A)
                .Subscribe(isPressed =>
                {
                    if (isPressed && moveDirectionFromKeyboard == Vector2.zero)
                        playerController.StartMoving();
                    
                    moveDirectionFromKeyboard.x += isPressed ? -1 : 1;
                    playerController.PutDirection(DirToDir((moveDirectionFromKeyboard.normalized, 1)));
                    
                    if (!isPressed && moveDirectionFromKeyboard == Vector2.zero)
                        playerController.StopMoving();
                })
                .AddTo(disposables);
            
            keyboardSession.OnKey(Key.D)
                .Subscribe(isPressed =>
                {
                    if (isPressed && moveDirectionFromKeyboard == Vector2.zero)
                        playerController.StartMoving();
                    
                    moveDirectionFromKeyboard.x += isPressed ? 1 : -1;
                    playerController.PutDirection(DirToDir((moveDirectionFromKeyboard.normalized, 1)));
                    
                    if (!isPressed && moveDirectionFromKeyboard == Vector2.zero)
                        playerController.StopMoving();
                })
                .AddTo(disposables);
            
            keyboardSession.OnKey(Key.W)
                .Subscribe(isPressed =>
                {
                    if (isPressed && moveDirectionFromKeyboard == Vector2.zero)
                        playerController.StartMoving();
                    
                    moveDirectionFromKeyboard.y += isPressed ? 1 : -1;
                    playerController.PutDirection(DirToDir((moveDirectionFromKeyboard.normalized, 1)));
                    
                    if (!isPressed && moveDirectionFromKeyboard == Vector2.zero)
                        playerController.StopMoving();
                })
                .AddTo(disposables);
            
            keyboardSession.OnKey(Key.S)
                .Subscribe(isPressed =>
                {
                    if (isPressed && moveDirectionFromKeyboard == Vector2.zero)
                        playerController.StartMoving();
                    
                    moveDirectionFromKeyboard.y += isPressed ? -1 : 1;
                    playerController.PutDirection(DirToDir((moveDirectionFromKeyboard.normalized, 1)));
                    
                    if (!isPressed && moveDirectionFromKeyboard == Vector2.zero)
                        playerController.StopMoving();
                })
                .AddTo(disposables);
        }

        private void SetupSession(SwipeGestureSession session, Stick stick)
        {
            gestureConnector.Connect(session)
                .AddTo(disposables);
            
            stick.Connect(session)
                .AddTo(disposables);

            session.OnDown
                .Subscribe(_ => playerController.StartMoving())
                .AddTo(disposables);
            
            session.OnUp
                .Subscribe(_ => playerController.StopMoving())
                .AddTo(disposables);
            
            stick.OnDirectionAndForce
                .ObserveOnMainThread()
                .Subscribe(data =>
                {
                    playerController.PutDirection(DirToDir(data));
                })
                .AddTo(disposables);
        }
        
        private void OnDisable()
        {
            disposables.Clear();
        }

        public Vector3 DirToDir((Vector2 Direction, float Force) data)
        {
            Vector3 cameraForward = new Vector3(camera.transform.forward.x, 0f, camera.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(camera.transform.right.x, 0f, camera.transform.right.z).normalized;
            Vector3 moveDirection = (cameraForward * data.Direction.y + cameraRight * data.Direction.x).normalized;
            return moveDirection * data.Force;
        }
    }
}