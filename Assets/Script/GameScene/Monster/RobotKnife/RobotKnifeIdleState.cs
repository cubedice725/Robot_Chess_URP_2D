public class RobotKnifeIdleState : IState
{
    MonsterMovement _monsterMovement;
    public RobotKnifeIdleState(MonsterMovement monsterMovement)
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
