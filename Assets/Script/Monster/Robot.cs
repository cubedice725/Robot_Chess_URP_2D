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
        if (GameManager.Instance.monsterTurn && start && GetComponent<Monster>().Authority)
        {
            // ��Ÿ� �ȿ� ������ ��ų �ƴϸ� ������
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

        // ���� ���� ������ �ѹ��� �۵�
        if (!GameManager.Instance.monsterTurn && !start)
        {
            state = State.Idle;
            start = true;
        }
    }
}
