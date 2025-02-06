using System;
using System.Collections.Generic;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace ExitTheBoard
{
    public class ScreenRaycaster
    {
        private readonly Dictionary<Vector3, RaycastHit> rayHits = new();

        private IDisposable nextFrame;
        private int usersCount;
        
        public IDisposable Enable()
        {
            return Disposable.Create(Clear);
        }
        
        public bool GetHit(out RaycastHit hit, Vector2 screenPosition, Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(screenPosition);

            if (rayHits.Count > 0 && rayHits.TryGetValue(ray.GetPoint(1000), out hit))
                return true;

            if (!Physics.Raycast(ray, out hit)) 
                return false;
            
            rayHits.AddOrReplace(ray.GetPoint(1000), hit);
            nextFrame ??= Observable.NextFrame(FrameCountType.FixedUpdate)
                .Subscribe(_ => Clear());
                    
            return true;
        }

        private void Clear()
        {
            rayHits.Clear();
            nextFrame?.Dispose();
            nextFrame = null;
        }
    }
}