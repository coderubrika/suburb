using System;
using UnityEngine;
using Zenject;

namespace ExitTheBoard
{
    public class LevelMono : MonoBehaviour
    {
        private RailsNavigator railsNavigator;
        
        [SerializeField] private Transform linesHolder;
        [SerializeField] private RiderLine[] riders;

        [Inject]
        private void Construct(RailsNavigator railsNavigator)
        {
            this.railsNavigator = railsNavigator;
        }
        
        public void Activate()
        {
            railsNavigator.Init(riders);
        }

        public void Deactivate()
        {
            railsNavigator.Clear();
        }
    }
    
    [Serializable]
    public struct RiderLine
    {
        public RailRider Rider;
        public LineNodeMono Line;
    }
}