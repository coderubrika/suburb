using System;
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
        private LayerOrderer layerOrderer;
        
        [SerializeField] PlayerHealthIndicator healthIndicator;
        [SerializeField] private Graphic playerBody;
        [SerializeField] private Graphic playerBodyBorder;
        [SerializeField] private Graphic background;

        [SerializeField] private float deltaThreshold;
        [SerializeField] private float deltaFactor;
        [SerializeField] private Rigidbody2D rigidbody;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private float forceFactor;
        [SerializeField] private float slowFactor;
        [SerializeField] private float anchorFactor;
        [SerializeField] private float health;
        [SerializeField] private float damageFactor;
        
        private readonly CompositeDisposable disposables = new();
        private readonly CompositeDisposable destroyDisposables = new();
        
        private RectTransform playerBodyTransform;
        public PlayerController PlayerController {get; private set;}

        public float DamageFactor => damageFactor;
        public PlayerHealthIndicator HealthIndicator => healthIndicator;
        public float Health => health;
        public float AnchorFactor => anchorFactor;
        public float SlowFactor => slowFactor;
        public float ForceFactor => forceFactor;
        public float DeltaThreshold => deltaThreshold;
        public float DeltaFactor => deltaFactor;
        public Rigidbody2D Rigidbody => rigidbody;
        public BattleSide Side {get; private set;}
        public RectBasedSession InputSession {get; private set;}
        public PlayerData PlayerData {get; private set;}
        public CircleCollider2D CircleCollider => circleCollider;
        
        [Inject]
        private void Construct(
            BattleService battleService,
            InjectCreator injectCreator,
            LayerOrderer layerOrderer)
        {
            this.battleService = battleService;
            this.layerOrderer = layerOrderer;
            
            InputSession = new RectBasedSession(transform as RectTransform);
            InputSession.SetBookResources(true);
            InputSession.SetPreventNext(true);
                    
            playerBodyTransform = transform as RectTransform;
            var touchCompositor = injectCreator.Create<OneTouchPluginCompositor>();
            InputSession.AddCompositor(touchCompositor)
                .AddTo(destroyDisposables);
                    
            var swipeTouchPlugin = injectCreator.Create<OneTouchSwipePlugin>();
            touchCompositor.Link<SwipeMember>(swipeTouchPlugin)
                .AddTo(destroyDisposables);
            
            PlayerController = injectCreator.Create<PlayerController>(this);
        }

        public void AddTo(IDisposable disposable)
        {
            disposables.Add(disposable);
        }
        
        public void OnCollisionEnter2D(Collision2D other)
        {
            PlayerController.CalcContact(other);
        }

        private void SetupCreate()
        {
            float dpiFactor = Screen.dpi/500;
            playerBodyTransform.sizeDelta *= dpiFactor;
        }
        
        private void Setup(BattleSide side, PlayerData data)
        {
            PlayerData = data;
            circleCollider.radius = playerBodyTransform.rect.width / 2;
            
            Side = side;
            healthIndicator.SetHealthPercentage(1);
            
            playerBody.color = data.BodyColor;
            playerBodyBorder.color = data.BodyBorderColor;
            background.color = data.BackgroundColor;
            
            transform.localRotation = side == BattleSide.Bottom 
                ? Quaternion.identity 
                : Quaternion.Euler(0f, 0f, 180f);
            
            
            PlayerController.Start();
            
            layerOrderer.ConnectFirst(InputSession)
                .AddTo(disposables);
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
        
        public class Pool : MonoMemoryPool<BattleSide, PlayerData ,PlayerView>
        {
            protected override void Reinitialize(BattleSide side, PlayerData data, PlayerView item)
            {
                item.Setup(side, data);
            }

            protected override void OnCreated(PlayerView item)
            {
                base.OnCreated(item);
                item.SetupCreate();
            }

            protected override void OnDespawned(PlayerView item)
            {
                item.PlayerController.Clear();
                item.disposables.Clear();
                base.OnDespawned(item);
            }

            protected override void OnDestroyed(PlayerView item)
            {
                item.destroyDisposables.Dispose();
                base.OnDestroyed(item);
            }
        }
    }
}