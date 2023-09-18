using Suburb.Inputs;
using Suburb.Detectors;

namespace Suburb.Common
{
    public class TravelingState : IGameState
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly IGestureProvider gestureProvider;
        private readonly WorldCameraController worldCameraController;
        private readonly PickDetector pickDetector;

        public TravelingState(
            IGestureProvider gestureProvider,
            WorldCameraController worldCameraController,
            PickDetector pickDetector,
            GameStateMachine gameStateMachine)
        {
            this.gestureProvider = gestureProvider;
            this.worldCameraController = worldCameraController;
            this.pickDetector = pickDetector;
            this.gameStateMachine = gameStateMachine;
        }

        public void Continue()
        {
            gestureProvider.Enable();
            worldCameraController.Enable();
            pickDetector.Enable();
        }

        public void Pause()
        {
            gestureProvider.Disable();
            worldCameraController.Disable();
            pickDetector.Disable();
        }

        public void Disable()
        {
            gestureProvider.Disable();
            worldCameraController.Disable();
            pickDetector.Disable();
        }

        public void Enable()
        {
            gestureProvider.Enable();
            worldCameraController.Enable();
            pickDetector.Enable();
        }
    }
}
