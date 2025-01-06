using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.Battle.UI
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    
    public class PlayerView : MonoBehaviour
    {
        private BattleService battleService;
        private InjectCreator injectCreator;
        private LayerOrderer layerOrderer;
        
        [SerializeField] PlayerHealthIndicator healthIndicator;
        [SerializeField] private Graphic playerBody;
        [SerializeField] private Graphic playerBodyBorder;
        [SerializeField] private Graphic background;

        [SerializeField] private float deltaThreshold;
        [SerializeField] private float deltaFactor;
        [SerializeField] private Rigidbody2D rigidbody;
        [SerializeField] private float forceFactor;
        private readonly CompositeDisposable disposables = new();
        
        private RectBasedSession inputSession;
        private BattleSide battleSide;
        private RectTransform playerBodyTransform;
        private Vector2 accumulatedDelta;
        
        [Inject]
        private void Construct(
            BattleService battleService,
            InjectCreator injectCreator,
            LayerOrderer layerOrderer)
        {
            this.battleService = battleService;
            this.injectCreator = injectCreator;
            this.layerOrderer = layerOrderer;
            
            inputSession = new RectBasedSession(transform as RectTransform);
            inputSession.SetBookResources(true);
            inputSession.SetPreventNext(true);
                    
            playerBodyTransform = transform as RectTransform;
            var touchCompositor = injectCreator.Create<OneTouchPluginCompositor>();
            inputSession.AddCompositor(touchCompositor)
                .AddTo(disposables);
                    
            var swipeTouchPlugin = injectCreator.Create<OneTouchSwipePlugin>();
            touchCompositor.Link<SwipeMember>(swipeTouchPlugin)
                .AddTo(disposables);
            
        }
        
        private void Setup(BattleSide side)
        {
            accumulatedDelta = Vector2.zero;
            battleService.RegisterPlayer(this, side);
            battleSide = side;
            playerBody.color = Random.ColorHSV(0.7f, 1f, 0.7f, 1f, 0, 1f);
            playerBodyBorder.color = Random.ColorHSV(0f, 0.7f, 0.7f, 1f, 0, 1f);
            background.color = battleService.GetColor(side);
            healthIndicator.SetHealthPercentage(1);
            
            transform.localRotation = side == BattleSide.Bottom 
                ? Quaternion.identity 
                : Quaternion.Euler(0f, 0f, 180f);
            
            inputSession.GetMember<SwipeMember>().OnDrag
                .Subscribe(HandleDrag)
                .AddTo(disposables);
                    
            layerOrderer.ConnectFirst(inputSession)
                .AddTo(disposables);
        }

        private void HandleDrag(Vector2 delta)
        {
            // допустим я использую физику
            
            //transform.position = ClampPosition(transform.position + delta.To3());
            //transform.position += delta.To3();
            accumulatedDelta += delta;
            float deltaDistance = battleService.BattleZone.InverseTransformVector(delta).magnitude;
            if (deltaDistance < deltaThreshold)
                return;
            
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg; // вычисляем угол в радианах и переводим в градусы
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.Euler(0, 0, angle - 90),  // Поворот только по оси Z, так как Canvas работает в 2D
                Time.deltaTime * deltaDistance * deltaFactor
            );
        }

        private void FixedUpdate()
        {
            rigidbody.AddForce(battleService.BattleZone.InverseTransformVector(accumulatedDelta) * forceFactor);
            accumulatedDelta = Vector2.zero;
        }
        
        private Vector3 ClampPosition(Vector3 position)
        {
            float radius = GetRadius();
            position.x = Mathf.Clamp(position.x, radius, Screen.width - radius);
            position.y = Mathf.Clamp(position.y, radius, Screen.height - radius);
            position.z = 0;
            return position;
        }

        private float GetRadius()
        {
            return battleService.BattleZone.TransformVector(Vector3.right * playerBodyTransform.rect.width * 0.5f).x;
        }
        
        public class Pool : MonoMemoryPool<BattleSide,PlayerView>
        {
            protected override void Reinitialize(BattleSide side, PlayerView item)
            {
                item.Setup(side);
            }

            protected override void OnDespawned(PlayerView item)
            {
                item.disposables.Clear();
                base.OnDespawned(item);
            }
        }
    }
}