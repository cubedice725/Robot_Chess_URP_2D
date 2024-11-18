using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(RobotSkillCastingState))]
public class Robot : Monster
{
    public override int MovingDistance { get => 1; set { } }
    public override float MoveSpeed { get => 0.7f; set { } }

    public bool start = true;

    protected override void Awake()
    {
        base.Awake();
        monsterIdleState = new RobotIdleState(monsterMovement);
        monsterMovingState = new RobotMovingState(monsterMovement);
        monsterSkillCastingState = GetComponent<RobotSkillCastingState>();
        monsterStateMachine.Initialize(monsterIdleState);
    }

    protected override void UpdateMonster()
    {
        // 몬스터 턴이되면 한번만 작동, Authority를 통해 권한이 있을경우 움직임
        if (GameManager.Instance.monsterTurn && start && Authority)
        {
            monsterMovement.MonsterPathFinding();
            // 사거리 안에 있으면 스킬 아니면 움직임
            if (monsterMovement.AttackNavigation())
            {
                if (AttackCount == 1)
                {
                    TurnPass();
                    return;
                }

                state = GameManager.State.Skill;
                AttackCount++;
            }
            else
            {
                if (MoveCount == 1)
                {
                    TurnPass();
                    return; 
                }
                state = GameManager.State.Move;
                MoveCount++;
            }
            start = false;
        }
        if (Flag)
        {
            start = true;
            state = GameManager.State.Idle;
        }
    }
}
