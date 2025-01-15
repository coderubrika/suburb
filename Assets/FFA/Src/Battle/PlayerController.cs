using System;
using FFA.Battle.UI;
using Suburb.Inputs;
using UniRx;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace FFA.Battle
{
    public class PlayerController
    {
        private readonly PlayerView view;
        private readonly BattleService battleService;
        
        private Vector2 accumulatedDelta;
        private Vector2 accumulatedDeltaOld;
        private bool isControlBlocked;
        private Transform anchorBackTransform;
        private bool isMoveToAnchorBack;
        private bool isDown;

        private Vector2 velocity;
        private float currentHealth;

        public bool IsInStun { get; private set; }

        public Vector2 Velocity => velocity;

        public ReactiveCommand OnDead { get; } = new();
        
        public PlayerController(
            BattleService battleService,
            PlayerView view)
        {
            this.battleService = battleService;
            this.view = view;
        }
        
        public void Start()
        {
            currentHealth = view.Health;
            accumulatedDelta = Vector2.zero;
            IDisposable dragDisposable = view.InputSession.GetMember<SwipeMember>().OnDrag
                .Subscribe(HandleDrag);
            view.AddTo(dragDisposable);
            
            IDisposable downDisposable = view.InputSession.GetMember<SwipeMember>().OnDown
                .Subscribe(_ => isDown = true);
            view.AddTo(downDisposable);

            IDisposable upDisposable = view.InputSession.GetMember<SwipeMember>().OnUp
                .Subscribe(_ => isDown = false);
            view.AddTo(upDisposable);
            
            IDisposable fixedUpdateDisposable = Observable.EveryFixedUpdate()
                .Subscribe(_ => FixedUpdate());
            view.AddTo(fixedUpdateDisposable);
        }

        public void SetAnchorBackTransform(Transform anchorBackTransform)
        {
            this.anchorBackTransform = anchorBackTransform;
        }

        public void CalcContact(Collision2D other)
        {
            PlayerView otherPlayer = other.gameObject.GetComponent<PlayerView>();
            if (otherPlayer == null || otherPlayer.Side == view.Side)
                return;

            Vector2 velocityOtherRaw = otherPlayer.PlayerController.Velocity;
            Vector2 velocityOther = battleService.BattleZone.InverseTransformVector(velocityOtherRaw) * view.DamageFactor;
            Vector2 velocitySelf = battleService.BattleZone.InverseTransformVector(Velocity) * view.DamageFactor;

            if (otherPlayer.PlayerController.IsInStun)
                return;
            
            if (velocityOther.sqrMagnitude > velocitySelf.sqrMagnitude)
            {
                float velocityOtherMagnitude = velocityOther.magnitude;
                float velocitySelfMagnitude = velocitySelf.magnitude;
                bool isToStun = velocitySelfMagnitude / velocityOtherMagnitude < 0.2;
            
                SetDamage(velocityOtherMagnitude - velocitySelfMagnitude, isToStun);
                view.Rigidbody.velocity = velocityOtherRaw + Velocity;
            }
        }

        public void SetDamage(float damage, bool isSetToStun = true)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, view.Health);
            view.HealthIndicator.SetHealthPercentage(currentHealth / view.Health);
            IsInStun = isSetToStun;
            if (currentHealth == 0)
                OnDead.Execute();
        }
        
        public void BlockControl(bool isBlocking)
        {
            isControlBlocked = isBlocking;
        }

        public void SetMoveToAnchorBack(bool isMoveToAnchorBack)
        {
            this.isMoveToAnchorBack = isMoveToAnchorBack;
            if (this.isMoveToAnchorBack)
                IsInStun = true;
        }
        
        public void Clear()
        {
            velocity = Vector2.zero;
            IsInStun = false;
            isMoveToAnchorBack = false;
            isControlBlocked = false;
            accumulatedDelta = Vector2.zero;
            anchorBackTransform = null;
        }
        
        private void FixedUpdate()
        {
            velocity = view.Rigidbody.velocity;
            
            if (isMoveToAnchorBack)
            {
                Vector3 toAnchorNormalized = (anchorBackTransform.position - view.transform.position).normalized;
                view.Rigidbody.velocity = battleService.BattleZone.TransformVector(toAnchorNormalized * view.AnchorFactor);
            }
            else
            {
                if (isDown)
                {
                    IsInStun = false;
                    float accumulatedDeltaMagnitude = accumulatedDelta.magnitude;
                    float accumulatedDeltaOldMagnitude = accumulatedDeltaOld.magnitude;
                
                    if (accumulatedDeltaMagnitude > 0 && accumulatedDeltaOldMagnitude > 0)
                    {
                        float relation = accumulatedDeltaMagnitude / accumulatedDeltaOldMagnitude;
                        view.Rigidbody.velocity = Vector2.Lerp(view.Rigidbody.velocity, view.Rigidbody.velocity * relation, Time.fixedDeltaTime * view.SlowFactor);
                    }

                    if (accumulatedDeltaMagnitude == 0)
                    {
                        view.Rigidbody.velocity = Vector2.Lerp(view.Rigidbody.velocity, view.Rigidbody.velocity * 0.1f, Time.fixedDeltaTime * view.SlowFactor);
                    }

                }

                float forceFactor = battleService.BattleZone.TransformVector(Vector3.right * view.ForceFactor).x;
                view.Rigidbody.AddForce(accumulatedDelta * forceFactor);
            }
            
            accumulatedDeltaOld = accumulatedDelta;
            accumulatedDelta = Vector2.zero;
        }
        
        private void HandleDrag(Vector2 delta)
        {
            if (isControlBlocked)
                return;
            
            accumulatedDelta += delta;
            float deltaDistance = battleService.BattleZone.InverseTransformVector(delta).magnitude;
            if (deltaDistance < view.DeltaThreshold)
                return;
            
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg; // вычисляем угол в радианах и переводим в градусы
             view.transform.rotation = Quaternion.Slerp(
                 view.transform.rotation, 
                 Quaternion.Euler(0, 0, angle - 90),  // Поворот только по оси Z, так как Canvas работает в 2D
                 //Time.deltaTime * deltaDistance * view.DeltaFactor
                 Time.deltaTime * view.DeltaFactor
            );
        }
    }
}