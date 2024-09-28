public class MonsterBasicMovingState : MonsterMovingState
{
    public override void Entry()
    {
    }
    public override void IStateUpdate()
    {
        if (!monsterMovement.UpdateMove())
        {
            monster.monsterState = Monster.MonsterState.Idle;
            monster.flag = true;
        }
    }
    public override void Exit()
    {

    }

    

}
