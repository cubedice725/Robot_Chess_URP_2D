public class RobotSniperMovingState : IState
{
    MonsterMovement _monsterMovement;
    public RobotSniperMovingState(MonsterMovement monsterMovement)
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
            _monsterMovement.IdleState();
        }
    }
    public bool Exit()
    {
        return true;
    }
}