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
        [SerializeField] private Transform card;
        [SerializeField] private LineTrack track;
        
        private readonly CompositeDisposable disposables = new();

        private bool isMoved;
        private Vector2 screenPosition;
        private Vector3 offset;
        private void Awake()
        { 
            layerOrderer = new();
            mouseProvider = new();
            mouseResourceDistributor = new(mouseProvider);
            session = new(frame);
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
                    if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.name == card.name)
                    {
                        isMoved = true;
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
                    Vector3 pos = card.transform.position;
                    Vector3 dirOne = track.DirectionOne;
                    float trackLen = track.Length;
                    Vector3 objDir = pos - track.StartPoint;
                    float objProj = Vector3.Dot(objDir, dirOne);
                    Vector3 objParallel = dirOne * objProj;
                    Vector3 objPerp = objDir - objParallel;
                    float deltaProj = Vector3.Dot(dirOne, delta);
                    float deltaByObjProj = deltaProj + objProj;
                    float clampedProj = Mathf.Clamp(deltaByObjProj, 0, trackLen);
                    Vector3 newObjParallel = dirOne * clampedProj;
                    Vector3 finalPos = track.StartPoint + newObjParallel + objPerp;
                    card.transform.position = finalPos;
                })
                .AddTo(disposables);
            
            swipe.OnUp
                .Subscribe(pos =>
                {
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
