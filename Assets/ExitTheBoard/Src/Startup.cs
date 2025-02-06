using System.Collections.Generic;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace ExitTheBoard
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private LineNodeMono lineNodeMono;
        [SerializeField] private Card card;
        [SerializeField] private Card rotator;
        
        private readonly CompositeDisposable disposables = new();
        
        [Inject]
        private void Construct()
        {
            lineNodeMono.Scan();
        }
        
        private void OnEnable()
        {
            card.Activate();
            rotator.Activate();
        }

        private void OnDisable()
        {
            card.Deactivate();
            rotator.Deactivate();
            disposables.Clear();
        }
    }
}
