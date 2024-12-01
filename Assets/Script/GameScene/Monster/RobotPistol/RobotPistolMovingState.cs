public class RobotPistolMovingState : IState
{
    MonsterMovement _monsterMovement;
    public RobotPistolMovingState(MonsterMovement monsterMovement)
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