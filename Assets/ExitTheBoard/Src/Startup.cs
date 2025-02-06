using System.Collections.Generic;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace ExitTheBoard
{
    public class Startup : MonoBehaviour
    {
        private LayerOrderer layerOrderer;
        private InjectCreator injectCreator;
        private GORectSession cardSession;
        
        [SerializeField] private RectTransform frame;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PointsAnchorMono pointsAnchor;

        [SerializeField] private Card card;
        [SerializeField] private Rotator rotator;
        
        private readonly Dictionary<GameObject, object> interactables = new();
        private readonly CompositeDisposable disposables = new();
        private PointNode pointNode;
        private int endIndex;
        private LineTrack track;
        
        private bool isMoved;
        private Vector2 screenPosition;
        private Vector3 offset;
        private IMovable movable;
        
        [Inject]
        private void Construct(
            LayerOrderer layerOrderer, 
            InjectCreator injectCreator)
        {
            this.layerOrderer = layerOrderer;
            this.injectCreator = injectCreator;
            interactables.Add(card.gameObject, card);
            interactables.Add(rotator.gameObject, rotator);
            cardSession = injectCreator.Create<GORectSession>(new GORectSessionParams
            {
                Camera = mainCamera,
                Target = card.gameObject
            });
            (pointNode, endIndex) = pointsAnchor.GetStartEndPoints();
            track = new LineTrack(pointNode.Position, pointNode.NeighboursPoints[endIndex].Position);
        }

        private void OnEnable()
        {
            MouseSwipeCompositor compositor = injectCreator.Create<MouseSwipeCompositor>(MouseButtonType.Left);
            cardSession.AddCompositor(compositor)
                .AddTo(disposables);
            layerOrderer.ConnectFirst(cardSession)
                .AddTo(disposables);
            
            SwipeMember swipe = cardSession.GetMember<SwipeMember>();
            swipe.OnDown
                .Subscribe(pos =>
                {
                    movable = card;
                    isMoved = movable != null;
                    screenPosition = pos;
                })
                .AddTo(disposables);
            
            swipe.OnDrag
                .Subscribe(screenDelta =>
                {
                    if (!isMoved)
                        return;
                    Vector2 newScreenPosition = screenPosition + screenDelta;
                    
                    Vector3 cardPositionStart = mainCamera.ScreenToWorldPoint(screenPosition);
                    Vector3 cardPositionEnd = mainCamera.ScreenToWorldPoint(newScreenPosition);
                    
                    Vector3 delta = cardPositionEnd - cardPositionStart;
                    Vector3 dirOne = track.DirectionOne;
                    float deltaProj = Vector3.Dot(dirOne, delta);
                    Vector3 pos = movable.Transform.position;

                    Vector3 dir = deltaProj > 0 ? track.Direction : -track.Direction;
                    int sign = Vector3.Dot(dir, movable.Transform.forward) > 0 ? 1 : -1;
                    Vector3 side = movable.Transform.forward * movable.BoxCollider.size.z * 0.5f * movable.Transform.localScale.z * sign;
                    
                    float trackLen = track.Length;
                    Vector3 objDir = pos + side - track.StartPoint;
                    float objProj = Vector3.Dot(objDir, dirOne);
                    Vector3 objParallel = dirOne * objProj;
                    Vector3 objPerp = objDir - objParallel;
                    float deltaByObjProj = deltaProj + objProj;
                    float clampedProj = Mathf.Clamp(deltaByObjProj, 0, trackLen);
                    Vector3 newObjParallel = dirOne * clampedProj;
                    Vector3 finalPos = track.StartPoint + newObjParallel + objPerp;
                    movable.Transform.position = finalPos - side;
                })
                .AddTo(disposables);
            
            swipe.OnUp
                .Subscribe(pos =>
                {
                    movable = null;
                    isMoved = false;
                })
                .AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Clear();
        }
    }
}
