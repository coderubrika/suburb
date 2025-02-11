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
        [SerializeField] private LevelMono levelPrefab;
        
        private readonly CompositeDisposable disposables = new();

        private LevelMono levelMono;
        
        [Inject]
        private void Construct(InjectCreator injectCreator)
        {
            levelMono = injectCreator.Create(levelPrefab, null);
        }
        
        private void OnEnable()
        {
            levelMono.Activate();
        }

        private void OnDisable()
        {
            levelMono.Deactivate();
            disposables.Clear();
        }
    }
}
