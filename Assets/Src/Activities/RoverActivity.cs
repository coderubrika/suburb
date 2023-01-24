using Suburb.Interactables;
using UnityEngine;

namespace Suburb.Activities
{
    public class RoverActivity
    {
        private Rover rover;

        public void SetRover(Rover rover)
        {
            this.rover = rover;
        }

        public void SetPositon(Vector3 position)
        {
            if (rover == null)
                return;

            rover.Move(new Vector3(position.x, 0f, position.z));
            rover = null;
        }
    }
}
