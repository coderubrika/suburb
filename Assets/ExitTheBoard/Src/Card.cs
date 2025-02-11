using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace ExitTheBoard
{
    public class Card : MonoBehaviour
    {
        private LayerOrderer layerOrderer;
        //private UnitsOnRailsStore unitsOnRailsStore;
        private GORectSession inputSession;
        private MouseSwipeCompositor mouseSwipeCompositor;
        private SwipeMember swipe;
        private Camera viewCamera;

        [SerializeField] private LineNodeMono lineNodeMono;
        [SerializeField] private BoxCollider boxCollider;

        private readonly CompositeDisposable disposables = new();
        private readonly string id = GeneralUtils.GetUID();
        
        private LineNode lineNode;
        private Vector2 screenPosition;
        private LineTrack fullTrack;
        
        public Transform Transform => transform;
        public BoxCollider BoxCollider => boxCollider;
        
        [Inject]
        private void Construct(
            LayerOrderer layerOrderer,
            UnitsOnRailsStore unitsOnRailsStore,
            InjectCreator injectCreator)
        {
            this.layerOrderer = layerOrderer;
            //this.unitsOnRailsStore = unitsOnRailsStore;
            viewCamera = Camera.main;
            
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
            lineNode = lineNodeMono.Scan();
            fullTrack = lineNode.GetCollinearLineTrack();
            //unitsOnRailsStore.SetUnitAtLine(lineNode, id, GetProjection(), GetSize());
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
                    Vector3 dirOne = fullTrack.DirectionOne;
                    float deltaProj = Vector3.Dot(dirOne, delta);
                    Vector3 pos = Transform.position;

                    Vector3 dir = deltaProj > 0 ? fullTrack.Direction : -fullTrack.Direction;
                    int sign = Vector3.Dot(dir, Transform.forward) > 0 ? 1 : -1;
                    Vector3 side = Transform.forward * BoxCollider.size.z * 0.5f * Transform.localScale.z * sign;
                    
                    Vector3 objDirWithSide = pos + side - fullTrack.StartPoint;
                    float objProjWithSide = Vector3.Dot(objDirWithSide, dirOne);
                    Vector3 objParallel = dirOne * objProjWithSide;
                    Vector3 objPerp = objDirWithSide - objParallel;
                    float deltaByObjProjWithSide = deltaProj + objProjWithSide;
                    
                    float clampMin, clampMax;
                    float objProj = GetProjection();
                    float deltaByObjProj = deltaProj + objProj;
                    (clampMin, clampMax) = GetClamp(deltaByObjProj);
                    float clampedProj = Mathf.Clamp(deltaByObjProjWithSide, clampMin, clampMax);
                    Vector3 newObjParallel = dirOne * clampedProj;
                    Vector3 finalPos = fullTrack.StartPoint + newObjParallel + objPerp;
                    Transform.position = finalPos - side;
                    //unitsOnRailsStore.SetUnitAtLine(lineNode, id, GetProjection(), GetSize());
                })
                .AddTo(disposables);
        }

        private float GetSize()
        {
            return BoxCollider.size.z * Transform.localScale.z;
        }

        private float GetProjection()
        {
            Vector3 pos = Transform.position;
            Vector3 objDir = pos - fullTrack.StartPoint;
            Vector3 dirOne = fullTrack.DirectionOne;
            float objProj = Vector3.Dot(objDir, dirOne);
            return objProj;
        }

        private (float Min, float Max) GetClamp(float proj)
        {
            float min = 0;
            float max = fullTrack.Length;

            //var units = unitsOnRailsStore.GetUnits(lineNode);
            
            // foreach (var unit in units)
            // {
            //     if (unit.Id == id)
            //         continue;
            //
            //     float half = unit.Size * 0.5f;
            //     float minCandidate = unit.Projection - half;
            //     float maxCandidate = unit.Projection + half;
            //
            //     if (minCandidate >= proj)
            //     {
            //         max = Mathf.Min(max, minCandidate);
            //     }
            //
            //     if (maxCandidate <= proj)
            //     {
            //         min = Mathf.Max(min, maxCandidate);
            //     }
            // }
            
            return (min, max);
        }
    }
}