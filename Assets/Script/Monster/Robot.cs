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
        // ���� ���̵Ǹ� �ѹ��� �۵�, Authority�� ���� ������ ������� ������
        if (GameManager.Instance.monsterTurn && start && Authority)
        {
            monsterMovement.MonsterPathFinding();
            // ��Ÿ� �ȿ� ������ ��ų �ƴϸ� ������
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
