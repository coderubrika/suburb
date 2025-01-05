namespace FFA.Battle
{
    public class BattleStateFactory
    {
        public TBattleState GetState<TBattleState>()
            where TBattleState : BattleState
        {
            return null;
        }
    }
}