public class RobotIdleState : IState
{
    MonsterMovement _monsterMovement;
    public RobotIdleState(MonsterMovement monsterMovement)
    {
        _monsterMovement = monsterMovement;
    }
    public void Entry()
    {
    }
    public void IStateUpdate()
    {
        _monsterMovement.LookPlayerAnimation();
    }
    public bool Exit()
    {
        return false;
    }
}
