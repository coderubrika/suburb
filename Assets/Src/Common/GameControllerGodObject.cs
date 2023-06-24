using Suburb.Interactables;
using Suburb.Selectors;

namespace Suburb.Common
{
    public class GameControllerGodObject
    {
        private readonly InteractablesSelector interactablesSelector;
        private readonly WorldCameraController worldCameraController;
        private readonly Rover rover;
        private readonly Land land;

        public GameControllerGodObject(
            InteractablesSelector interactablesSelector,
            WorldCameraController worldCameraController,
            Land land,
            Rover rover)
        {
            this.interactablesSelector = interactablesSelector;
            this.worldCameraController = worldCameraController;
            this.land = land;
            this.rover = rover;
        }

        public void Start()
        {
            rover.Install();
            land.Install();
            worldCameraController.Enable();
            interactablesSelector.SetEnableChecks(true);
        }

        public void Stop()
        {
            rover.Uninstall();
            land.Uninstall();
            worldCameraController.Disable();
            interactablesSelector.SetEnableChecks(false);
        }
    }
}
