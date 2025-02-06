using System;
using Suburb.Inputs;
using UnityEngine;
using Zenject;

namespace ExitTheBoard
{
    public class GORectSession : RectBasedSession
    {
        private readonly ScreenRaycaster screenRaycaster;
        private readonly GameObject target;
        private readonly Camera camera;
        private readonly RaycastMember raycastMember;
        
        public GORectSession(
            ScreenRaycaster screenRaycaster,
            Camera camera,
            GameObject target,
            [InjectOptional] RectTransform bounds,
            [InjectOptional] RectTransform[] excludedRectsTransforms = null) : 
            base(bounds, excludedRectsTransforms)
        {
            this.screenRaycaster = screenRaycaster;
            this.target = target;
            this.camera = camera;
            raycastMember = GetMember<RaycastMember>();
        }

        public override bool CheckIncludeInBounds(Vector2 point)
        {
            bool inBounds = base.CheckIncludeInBounds(point);
            
            if (!inBounds)
                return false;

            if (!screenRaycaster.GetHit(out RaycastHit hit, point, camera) && hit.collider.gameObject != target) 
                return false;
            
            raycastMember.PutHit(hit);
            return true;

        }

        IDisposable Enable()
        {
            return screenRaycaster.Enable();
        }
    }
}