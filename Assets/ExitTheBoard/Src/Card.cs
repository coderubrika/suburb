using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace ExitTheBoard
{
    public class Card : MonoBehaviour, IMovable
    {
        private LayerOrderer layerOrderer;
        private GORectSession inputSession;
        private MouseSwipeCompositor mouseSwipeCompositor;
        private SwipeMember swipe;
        private Camera viewCamera;
        
        [SerializeField] private BoxCollider boxCollider;

        private readonly CompositeDisposable disposables = new();
        
        private Vector2 screenPosition;
        private LineTrack track;
        
        public Transform Transform => transform;
        public BoxCollider BoxCollider => boxCollider;
        
        [Inject]
        private void Construct(
            LayerOrderer layerOrderer,
            InjectCreator injectCreator)
        {
            this.layerOrderer = layerOrderer;
            viewCamera = Camera.main;
            
            //tmp for test
            track = new LineTrack(new Vector3(4.26f, 0, -9.11f), new Vector3(-4.21f, 0, -9.11f));
            
            inputSession = injectCreator.Create<GORectSession>(new GORectSessionParams
            {
                Camera = Camera.main,
                Target = gameObject
            });
            
            mouseSwipeCompositor = injectCreator.Create<MouseSwipeCompositor>(MouseButtonType.Left);
            swipe = inputSession.GetMember<SwipeMember>();
            inputSession.SetBookResources(true);
        }
        
        public void Activate()
        {
            inputSession.AddCompositor(mouseSwipeCompositor)
                .AddTo(disposables);
            layerOrderer.ConnectFirst(inputSession)
                .AddTo(disposables);
            
            SetupInputHandle();
        }

        public void Deactivate()
        {
            disposables.Clear();
        }

        private void SetupInputHandle()
        {
            swipe.OnDown
                .Subscribe(pos => screenPosition = pos)
                .AddTo(disposables);
            
            swipe.OnDrag
                .Subscribe(screenDelta =>
                {
                    Vector2 newScreenPosition = screenPosition + screenDelta;
                    
                    Vector3 cardPositionStart = viewCamera.ScreenToWorldPoint(screenPosition);
                    Vector3 cardPositionEnd = viewCamera.ScreenToWorldPoint(newScreenPosition);
                    
                    Vector3 delta = cardPositionEnd - cardPositionStart;
                    Vector3 dirOne = track.DirectionOne;
                    float deltaProj = Vector3.Dot(dirOne, delta);
                    Vector3 pos = Transform.position;

                    Vector3 dir = deltaProj > 0 ? track.Direction : -track.Direction;
                    int sign = Vector3.Dot(dir, Transform.forward) > 0 ? 1 : -1;
                    Vector3 side = Transform.forward * BoxCollider.size.z * 0.5f * Transform.localScale.z * sign;
                    
                    float trackLen = track.Length;
                    Vector3 objDir = pos + side - track.StartPoint;
                    float objProj = Vector3.Dot(objDir, dirOne);
                    Vector3 objParallel = dirOne * objProj;
                    Vector3 objPerp = objDir - objParallel;
                    float deltaByObjProj = deltaProj + objProj;
                    float clampedProj = Mathf.Clamp(deltaByObjProj, 0, trackLen);
                    Vector3 newObjParallel = dirOne * clampedProj;
                    Vector3 finalPos = track.StartPoint + newObjParallel + objPerp;
                    Transform.position = finalPos - side;
                })
                .AddTo(disposables);
        }
    }
}