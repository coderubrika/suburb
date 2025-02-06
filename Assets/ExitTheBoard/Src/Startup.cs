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
