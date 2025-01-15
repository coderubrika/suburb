using System;
using System.Linq;
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
        private Camera camera;
        private KeyboardInputProvider keyboardInputProvider;
        private InjectCreator injectCreator;
        private LayerOrderer layerOrderer;
        //private MouseProvider mouseProvider;
        
        [SerializeField] private Stick joystick;
        [SerializeField] private RectTransform joystickArea;
        
        [SerializeField] private Stick joystick1;
        [SerializeField] private RectTransform joystickArea1;
        
        [SerializeField] private Stick joystick2;
        [SerializeField] private RectTransform joystickArea2;
        
        [SerializeField] private RectTransform[] zoomAreas;
        
        private readonly CompositeDisposable disposables = new();
        private Vector2 moveDirectionFromKeyboard;
        
        [Inject]
        private void Construct(
            PlayerController playerController, 
            Camera camera,
            KeyboardInputProvider keyboardInputProvider,
            InjectCreator injectCreator,
            LayerOrderer layerOrderer)
        {
            this.playerController = playerController;
            this.camera = camera;
            this.keyboardInputProvider = keyboardInputProvider;
            this.injectCreator = injectCreator;
            this.layerOrderer = layerOrderer;
        }
        
        private void OnEnable()
        {
            SetupSession(new RectBasedSession(joystickArea), joystick);
            SetupSession(new RectBasedSession(joystickArea1), joystick1);
            SetupSession(new RectBasedSession(joystickArea2), joystick2);
            foreach (var zoomArea in zoomAreas)
                SetupZoomSession(zoomArea);
            
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

        private void SetupSession(CompositorsSession session, Stick stick)
        {
            // technical gesture logic
            // var compositor = injectCreator.Create<OneTouchPluginCompositor>();
            // var swipePlugin = injectCreator.Create<OneTouchSwipePlugin>();
            
            // session.AddCompositor(compositor)
            //     .AddTo(disposables);
            
            var mouseSwipeCompositor = injectCreator.Create<MouseSwipeCompositor>(MouseButtonType.Left);
            session.AddCompositor(mouseSwipeCompositor)
                .AddTo(disposables);
            
            // var mouseZoomCompositor = injectCreator.Create<MouseZoomCompositor>();
            // session.AddCompositor(mouseZoomCompositor)
            //     .AddTo(disposables);
            //
            // compositor.Link<SwipeMember>(swipePlugin)
            //     .AddTo(disposables);
            
            session.SetBookResources(true);
            
            layerOrderer.ConnectFirst(session)
                .AddTo(disposables);

            // business gesture login
            var swipe = session.GetMember<SwipeMember>();
            stick.Connect(swipe)
                .AddTo(disposables);
            
            swipe.OnDown
                .Subscribe(_ => playerController.StartMoving())
                .AddTo(disposables);
            
            swipe.OnUp
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

        private void SetupZoomSession(RectTransform rectTransform)
        {
            var session = new RectBasedSession(rectTransform);
            var compositor = injectCreator.Create<OneTwoTouchPluginCompositor>();
            
            var swipePlugin = injectCreator.Create<OneTwoTouchSwipePlugin>();
            var zoomPlugin = injectCreator.Create<TwoTouchZoomPlugin>();
            var rotatePlugin = injectCreator.Create<TwoTouchRotatePlugin>();
            
            session.AddCompositor(compositor)
                .AddTo(disposables);
            
            compositor.Link<SwipeMember>(swipePlugin)
                .AddTo(disposables);
            compositor.Link<ZoomMember>(zoomPlugin)
                .AddTo(disposables);
            compositor.Link<RotateMember>(rotatePlugin)
                .AddTo(disposables);
            
            var mouseSwipeCompositor = injectCreator.Create<MouseSwipeCompositor>(MouseButtonType.Left);
            session.AddCompositor(mouseSwipeCompositor)
                .AddTo(disposables);
            
            var mouseZoomCompositor = injectCreator.Create<MouseZoomCompositor>();
            session.AddCompositor(mouseZoomCompositor)
                .AddTo(disposables);
            
            session.SetBookResources(true);
            
            layerOrderer.ConnectFirst(session)
                .AddTo(disposables);
            
            var zoom = session.GetMember<ZoomMember>();
            var swipe = session.GetMember<SwipeMember>();
            var rotate = session.GetMember<RotateMember>();
            
            zoom.OnZoom
                .Subscribe(zoomData =>
                {
                    var zoomCenterPosition = zoomData.Position.To3();
                    Vector3 offset = zoomCenterPosition - rectTransform.position;
                    rectTransform.localScale *= zoomData.Zoom;
                    rectTransform.position = zoomCenterPosition - offset * zoomData.Zoom;
                })
                .AddTo(disposables);
            
            swipe.OnDragStart
                .Merge(swipe.OnDrag)
                .Subscribe(delta =>
                {
                    rectTransform.position += delta.To3();
                })
                .AddTo(disposables);
            
            rotate.OnRotate
                .Subscribe(angle =>
                {
                    rectTransform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);
                })
                .AddTo(disposables);
        }
    }
}