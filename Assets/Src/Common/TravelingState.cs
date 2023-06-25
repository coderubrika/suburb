using Suburb.Core.Inputs;

namespace Suburb.Common
{
    public class TravelingState : IGameState
    {
        private readonly IGestureProvider gestureProvider;
        private readonly WorldCameraController worldCameraController;

        public TravelingState(
            IGestureProvider gestureProvider,
            WorldCameraController worldCameraController)
        {
            this.gestureProvider = gestureProvider;
            this.worldCameraController = worldCameraController;
        }

        public void Disable()
        {
            gestureProvider.Disable();
            worldCameraController.Disable();
        }

        public void Enable()
        {
            gestureProvider.Enable();
            worldCameraController.Enable();
        }
    }
}
