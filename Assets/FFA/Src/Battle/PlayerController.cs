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
        
        public PlayerController(
            BattleService battleService,
            PlayerView view)
        {
            this.battleService = battleService;
            this.view = view;
        }
        
        public void Start()
        {
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
        
        public void BlockControl(bool isBlocking)
        {
            isControlBlocked = isBlocking;
        }

        public void SetMoveToAnchorBack(bool isMoveToAnchorBack)
        {
            this.isMoveToAnchorBack = isMoveToAnchorBack;
        }
        
        public void Clear()
        {
            isMoveToAnchorBack = false;
            isControlBlocked = false;
            accumulatedDelta = Vector2.zero;
            anchorBackTransform = null;
        }
        
        private void FixedUpdate()
        {
            if (isMoveToAnchorBack)
            {
                Vector3 toAnchorNormalized = (anchorBackTransform.position - view.transform.position).normalized;
                view.Rigidbody.AddForce(toAnchorNormalized * view.ForceFactor);
            }
            else
            {
                if (isDown)
                {
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
                
                
                Vector3 delta = battleService.BattleZone.InverseTransformVector(accumulatedDelta);
                view.Rigidbody.AddForce(delta * view.ForceFactor);
                
                // если длинна дельны увеличивается нужно добавлять как это происходит если длинна дельны уменьшается
                // нужно уменьшать скорость пропрорционально отношению дельт
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
                Time.deltaTime * deltaDistance * view.DeltaFactor
            );
        }
    }
}