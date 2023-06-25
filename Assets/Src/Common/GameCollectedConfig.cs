namespace Suburb.Common
{
    public class GameCollectedConfig
    {
        public GameCollectedConfig(GameCollectedData gameCollectedData)
        {
            Id = gameCollectedData.Id;
        }

        public int Id { get; private set; }
    }
}
