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
            // а что если сделать так зарегистировать всех сущностей в сервисе сущностей
            // при загрузке уровня подцепить скрипт настройки уровня
            // который мы зальем в конфиг
            // он определит
            // если уйти от ecs то что получается
        }

        private void OnDisable()
        {
            levelMono.Deactivate();
            disposables.Clear();
        }
    }
}
