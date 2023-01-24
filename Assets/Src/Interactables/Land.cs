using Suburb.Activities;
using Suburb.Utils;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Suburb.Interactables
{
    public class Land : InteractableObject
    {
        private RoverActivity roverActivity;

        [Inject]
        public void Construct(RoverActivity roverActivity)
        {
            this.roverActivity = roverActivity;
        }

        public override void Interact(BaseInteractEventData data)
        {
            roverActivity.SetPositon(data.Ray.GetPoint(data.Distance));
        }
    }
}