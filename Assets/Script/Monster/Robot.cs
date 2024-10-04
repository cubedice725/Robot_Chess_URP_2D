using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(RobotSkillCastingState))]
public class Robot : Monster
{
    bool start = true;
    public override void Awake()
    {
        base.Awake();
        monsterIdleState = new RobotIdleState(monsterMovement);
        monsterMovingState = new RobotMovingState(monsterMovement);
        monsterSkillCastingState = GetComponent<RobotSkillCastingState>();
        monsterStateMachine.Initialize(monsterIdleState);
    }
    public override void UpdateMonster()
    {
        if(GameManager.Instance.monsterTurn && start)
        {
            if (monsterMovement.AttackNavigation())
            {
                state = State.Skill;
            }
            else
            {
                state = State.Move;
            }
            start = false;
        }
        if (!GameManager.Instance.monsterTurn && !start)
        {
            state = State.Idle;
            start = true;
        }
    }
}
