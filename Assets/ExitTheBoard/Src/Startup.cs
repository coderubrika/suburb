using System.Collections.Generic;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace ExitTheBoard
{
    public class Startup : MonoBehaviour
    {
        private LayerOrderer layerOrderer;
        private MouseProvider mouseProvider;
        private MouseResourceDistributor mouseResourceDistributor;
        private RectBasedSession session;
        
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
        
        private void Awake()
        { 
            interactables.Add(card.gameObject, card);
            interactables.Add(rotator.gameObject, rotator);
            
            layerOrderer = new();
            mouseProvider = new();
            mouseResourceDistributor = new(mouseProvider);
            session = new(frame);
            (pointNode, endIndex) = pointsAnchor.GetStartEndPoints();
            track = new LineTrack(pointNode.Position, pointNode.NeighboursPoints[endIndex].Position);
        }

        private void OnEnable()
        {
            MouseSwipeCompositor compositor = new(mouseProvider, mouseResourceDistributor, MouseButtonType.Left);
            session.AddCompositor(compositor)
                .AddTo(disposables);
            layerOrderer.ConnectFirst(session)
                .AddTo(disposables);

            SwipeMember swipe = session.GetMember<SwipeMember>();
            swipe.OnDown
                .Subscribe(pos =>
                {
                    Ray ray = mainCamera.ScreenPointToRay(pos);
                    if (Physics.Raycast(ray, out RaycastHit hit) && interactables.TryGetValue(hit.collider.gameObject, out object interactable))
                    {
                        movable = interactable as IMovable;
                        isMoved = movable != null;
                        screenPosition = pos;
                    }
                })
                .AddTo(disposables);
            
            swipe.OnDrag
                .Subscribe(screenDelta =>
                {
                    if (!isMoved)
                        return;
                    Vector2 newScreenPosition = screenPosition + screenDelta;
                    
                    Vector3 cardPositionStart = UIUtils.TransformScreenToWorld(frame, mainCamera, screenPosition);
                    Vector3 cardPositionEnd = UIUtils.TransformScreenToWorld(frame, mainCamera, newScreenPosition);
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
