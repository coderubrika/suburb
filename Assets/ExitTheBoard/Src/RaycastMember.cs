using UniRx;
using UnityEngine;

namespace ExitTheBoard
{
    public class RaycastMember
    {
        public ReactiveCommand<RaycastHit> OnHit { get; } = new();
        
        public void PutHit(RaycastHit hit)
        {
            OnHit.Execute(hit);
        }
    }
}