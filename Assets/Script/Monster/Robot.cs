using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(RobotSkillCastingState))]
public class Robot : Monster
{
    public override int MovingDistance { get => movingDistance; set => movingDistance = value; }
    public override float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    private float moveSpeed = 0.7f;
    private int movingDistance = 1;

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
        // ���� ���̵Ǹ� �ѹ��� �۵�, Authority�� ���� ������ ������� ������
        if (GameManager.Instance.monsterTurn && start && Authority)
        {
            // ��Ÿ� �ȿ� ������ ��ų �ƴϸ� ������
            if (monsterMovement.AttackNavigation())
            {
                if (AttackCount == 1)
                {
                    TurnPass();
                    return;
                }

                state = State.Skill;
                AttackCount++;
            }
            else
            {
                if (MoveCount == 1)
                {
                    TurnPass();
                    return; 
                }
                state = State.Move;
                MoveCount++;
            }
            start = false;
        }
        if (Flag)
        {
            start = true;
            state = State.Idle;
        }
    }
}
