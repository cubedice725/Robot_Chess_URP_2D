using UnityEngine;
using static GameManager;


// ���� �߻��� ����ؾ��� ����
// Authority�� �ൿ�� �Ҵ��ߴ���
public class Monster : MonoBehaviour
{
    // ������ �ൿ�� ���� ��� True�� �ٲ����
    public bool Flag { get; set; } = false;

    // ���Ͱ� �̵��� ��ġ�� ���� �����ϱ� ���� ����
    // ��� �ൿ�� �ϱ� ���ؼ��� Authority�� true�� ���Ϳ��� �־����
    // �� ������ ������ ���Ͱ� �������� �ȴٴ� �ǹ�(��ų�� Authority�� ��������)
    public bool Authority { get; set; } = false;
    public int Num { get; set; }
    public virtual int AttackDistance { get; set; } = 1;
    public virtual int MovingDistance { get; set; } = 1;
    public virtual float MoveSpeed { get; set; } = 1f;
    public virtual float HP { get; set; } = 1;

    public bool Die { get; private set; } = false;

    public enum State
    {
        Idle,
        Move,
        Skill
    }
    public IState monsterSkillCastingState;
    public IState monsterMovingState;
    public IState monsterIdleState;

    public State state = State.Idle;

    public MonsterStateMachine monsterStateMachine;
    public MonsterMovement monsterMovement;

    public virtual void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
        monsterStateMachine = new MonsterStateMachine(this);
    }
    public void Update()
    {
        // �������
        if (HP <= 0 && !Die)
        {
            // ������ ������ �ش� �ڸ��� ���Ͱ� �ʿ� �����ִ°� �����ϱ� ���� ����
            if (state == State.Idle) 
            {
                Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.noting;
            }
            // ��� �ִϸ��̼��� �ݺ������� true�� ������ ���� �׷��⿡ ����� �ѹ����ؾ���
            Die = true;
            // ��� �ִϸ��̼ǰ� ���߿� �� �������� �ִ� ���͸� �ʿ��� ����
            monsterMovement.Die();
        }
        //���� �ϰ� ������� ������
        if (state == State.Idle)
        {
            monsterStateMachine.TransitionTo(monsterIdleState);
        }
        else if (state == State.Move)
        {
            monsterStateMachine.TransitionTo(monsterMovingState);
        }
        else if (state == State.Skill)
        {
            monsterStateMachine.TransitionTo(monsterSkillCastingState);
        }
        if (Die)
        {
            // ����ϸ� spawnMonsters�� �����ֱ� ������ �����̶� �÷��״� �׻� �������� �ѱ�� �ֵ�����
            Authority = false;
            Flag = true;
            return;
        }
        // ���� ���� ��� �����ڰ� �ۼ��Ͽ� ���� �������� ����
        UpdateMonster();

        monsterStateMachine.MonsterStateMachineUpdate();
    }
    public virtual void UpdateMonster() { }
}