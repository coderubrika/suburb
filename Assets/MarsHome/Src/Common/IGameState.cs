namespace Suburb.Common
{
    public interface IGameState
    {
        public void Enable();
        public void Disable();
        public void Pause();
        public void Continue();
    }
}
