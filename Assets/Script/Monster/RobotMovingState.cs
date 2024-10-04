public class RobotMovingState : IState
{
    MonsterMovement _monsterMovement;
    public RobotMovingState(MonsterMovement monsterMovement)
    {
        _monsterMovement = monsterMovement;
    }
    public void Entry()
    {
    }
    public void IStateUpdate()
    {
        if (!_monsterMovement.UpdateMove())
        {
            _monsterMovement.MonsterIdleState();
        }
    }
    public bool Exit()
    {
        return false;
    }
}
