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
                .Subscribe(delta =>
                {
                    if (!isMoved)
                        return;
                    Vector2 newScreenPosition = screenPosition + delta;
                    
                    Vector3 cardPositionStart = UIUtils.TransformScreenToWorld(frame, mainCamera, screenPosition);
                    Vector3 cardPositionEnd = UIUtils.TransformScreenToWorld(frame, mainCamera, newScreenPosition);
                    Vector3 cardDelta = cardPositionEnd - cardPositionStart;
                    //card.transform.position += cardDelta;
                    Vector3 cardPosition = card.transform.position;
                    Vector3 trackDirection = track.DirectionOne;
                    float projection = Vector3.Dot(cardDelta, trackDirection);
                    Vector3 projectedDelta = trackDirection * projection;
                    card.transform.position += projectedDelta;
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
