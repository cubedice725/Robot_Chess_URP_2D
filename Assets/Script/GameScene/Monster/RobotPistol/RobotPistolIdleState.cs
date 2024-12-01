public class RobotPistolIdleState : IState
{
    MonsterMovement _monsterMovement;
    public RobotPistolIdleState(MonsterMovement monsterMovement)
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
