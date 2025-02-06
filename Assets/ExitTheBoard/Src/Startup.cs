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
        [SerializeField] private PointsAnchorMono pointsAnchor;

        [SerializeField] private Card card;
        [SerializeField] private Card rotator;
        
        private readonly CompositeDisposable disposables = new();
        private PointNode pointNode;
        private int endIndex;
        private LineTrack track;
        
        [Inject]
        private void Construct()
        {
            (pointNode, endIndex) = pointsAnchor.GetStartEndPoints();
            track = new LineTrack(pointNode.Position, pointNode.NeighboursPoints[endIndex].Position);
            // как связать дороги
            // 1 любой обьект лежащий где то в рельсах должен заявить об этом
            // а конкретнее он должен заявить на какой линии он находится и в какой точке проекции он там находится
            // и еще какие у него границы относительно центра линии
            // далее он должен запросить на каждом кадре или подписаться на изменение своей линии и 
            // на подписке обновлять свои данные о линии
            // или что проще он должен просто обращаться к этому сервису чтобы получить текущую линию
            // тоесть он должен отправить себя и получить текущую линию
            // а чтобы отправить себя он должен себя зарегестрировать там изначально обьявив где он назодится
            // и какие у него границы
            // нужен сервис который владеет дорогой и может это как то организовать
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
