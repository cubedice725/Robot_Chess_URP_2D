public class RobotMovingState : IState
{
    MonsterMovement _monsterMovement;
    bool attack = false;
    public RobotMovingState(MonsterMovement monsterMovement)
    {
        _monsterMovement = monsterMovement;
    }
    public void Entry()
    {
        if (_monsterMovement.AttackNavigation())
        {
            attack = true;
        }
    }
    public void IStateUpdate()
    {
        if (attack)
        {
        }
    }
    public void Exit()
    {
    }
}
