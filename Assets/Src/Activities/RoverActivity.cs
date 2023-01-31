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
            Debug.Log("Rover setupped");
        }

        public void SetPositon(Vector3 position)
        {
            Debug.Log("Try setup for fover from land");
            if (rover == null)
                return;

            Debug.Log("Move rover");
            rover.Move(new Vector3(position.x, 0f, position.z));
            rover = null;
        }
    }
}
