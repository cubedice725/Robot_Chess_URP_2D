using UnityEngine;
using static GameManager;


// ���� �߻��� ����ؾ��� ����
// Authority�� �ൿ�� �Ҵ��ߴ���
public abstract class Monster : MonoBehaviour
{
    // ������ �ൿ�� ���� ��� True�� �ٲ����
    public bool Flag { get; set; } = false;

    // ���Ͱ� �̵��� ��ġ�� ���� �����ϱ� ���� ����
    // ��� �ൿ�� �ϱ� ���ؼ��� Authority�� true�� ���Ϳ��� �־����
    // �� ������ ������ ���Ͱ� �������� �ȴٴ� �ǹ�(��ų�� Authority�� ��������)
    public bool Authority  = false;
    public virtual int AttackDistance { get; set; } = 1;
    public virtual int MovingDistance { get; set; } = 1;
    public virtual float MoveSpeed { get; set; } = 1f;
    public virtual float HP { get; set; } = 1;
    public int MoveCount  = 0;
    public int AttackCount = 0;
    public bool Die { get; private set; } = false;
    public int ScorePoint { get; private set; } = 50;

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
    public BoxCollider2D boxCollider2D;
    public Rigidbody2D rigi2D;
    public virtual void Awake()
    {
        Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.moster;
        monsterMovement = GetComponent<MonsterMovement>();
        monsterStateMachine = new MonsterStateMachine(this);
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigi2D = GetComponent<Rigidbody2D>();
    }
    public void Update()
    {
        // ���� �ý����� Authority�� ���� Flag�� ����.
        // �� Authority�� True�̸� ������ False�� ���Ŀ� Flag�� True�� �ȴ�.
        // Authority, Flag�� �Ѵ� True�� �Ǵ°��� ����.
        // Die && Authority && Flag�� ��츦 ����� ���� GameManager�̴�.
        if (Die && Authority && Flag) return;

        // ��� ��쿡 ���� ��ȯ�� ���� �𸣱⿡ ���� ��ȯ�� ���� ���� �ؾ���
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
        // �������
        if (HP <= 0 && !Die)
        {
            // ������ ������ �ش� �ڸ��� ���Ͱ� �ʿ� �����ִ°� �����ϱ� ���� ����
            if (state == State.Idle) 
            {
                Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.noting;
            }
            boxCollider2D.enabled = false;
            rigi2D.simulated = false;

            Instance.GameScore += ScorePoint;
            // ��� �ִϸ��̼��� �ݺ������� true�� ������ ���� �׷��⿡ ����� �ѹ����ؾ���
            Die = true;
            // ��� �ִϸ��̼ǰ� ���߿� �� �������� �ִ� ���͸� �ʿ��� ����
            monsterMovement.Die();
            GetComponent<SpriteRenderer>().sortingOrder = 1;

        }

        // ��� ���� Authority�� ������ �̸� �ѱ������
        if (Die && Authority)
        {
            TurnPass();
        }

        // ������ ���º�ȯ�� ����� ����
        UpdateMonster();
        // ������ ���� Ŭ������ ����Ǿ�����
        // ex(MonsterStateMachine -> MonsterMovingState)
        monsterStateMachine.MonsterStateMachineUpdate();
    }
    public virtual void UpdateMonster() { }
    public void TurnPass()
    {
        Authority = false;
        Flag = true;
    }
}