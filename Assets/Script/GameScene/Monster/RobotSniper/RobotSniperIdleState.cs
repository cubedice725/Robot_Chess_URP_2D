public class RobotSniperIdleState : IState
{
    MonsterMovement _monsterMovement;
    public RobotSniperIdleState(MonsterMovement monsterMovement)
    {
        _monsterMovement = monsterMovement;
    }
    public void Entry()
    {
    }
    public void IStateUpdate()
    {
    }
    public bool Exit()
    {
        return false;
    }
}
