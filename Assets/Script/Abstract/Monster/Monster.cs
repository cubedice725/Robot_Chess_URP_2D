using UnityEngine;
using static GameManager;


// ���� �߻��� ����ؾ��� ����
// Authority�� �ൿ�� �Ҵ��ߴ���
public abstract class Monster : MonoBehaviour
{
    // ������ �ൿ�� ���� ��� True�� �ٲ����
    public bool Flag = false;

    // ���Ͱ� �̵��� ��ġ�� ���� �����ϱ� ���� ����
    // ��� �ൿ�� �ϱ� ���ؼ��� Authority�� true�� ���Ϳ��� �־����
    // �� ������ ������ ���Ͱ� �������� �ȴٴ� �ǹ�(��ų�� Authority�� ��������)
    public bool Authority  = false;
    public virtual int AttackDistance { get; set; } = 1;
    public virtual int MovingDistance { get; set; } = 1;
    public virtual float MoveSpeed { get; set; } = 1f;
    public virtual float HP { get; set; } = 1;
    public int MoveCount { get; set; } = 0;
    public int AttackCount { get; set; } = 0;
    public bool Die { get; private set; } = false;
    public int ScorePoint { get; private set; } = 50;
    public float time = 0;

    protected MonsterStateMachine monsterStateMachine;
    protected MonsterMovement monsterMovement;
    protected BoxCollider2D boxCollider2D;
    protected Rigidbody2D rigi2D;
    protected MyObject myObject;

    public State state { get;  set; } = State.Idle;
    protected IState monsterSkillCastingState;
    protected IState monsterMovingState;
    protected IState monsterIdleState;

    public virtual void Initialize()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 4;
        boxCollider2D.enabled = true;
        rigi2D.simulated = true;
        Flag = false;
        Authority = false;
        HP = 1;
        MoveCount = 0;
        AttackCount = 0;
        Die = false;
        time = 0;
    }
    protected virtual void Awake()
    {
        Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.moster;
        monsterMovement = GetComponent<MonsterMovement>();
        monsterStateMachine = new MonsterStateMachine(this);
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigi2D = GetComponent<Rigidbody2D>();
        myObject = GetComponent<MyObject>();
    }
    private void FixedUpdate()
    {
        if (Die && time >= 0)
        {
            time += Time.fixedDeltaTime;
        }
        if (time > 5 && Die)
        {
            GetComponent<MyObject>().Destroy();
            time = -1;
        }
    }
    protected void Update()
    {
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
    protected virtual void UpdateMonster() { }
    protected void TurnPass()
    {
        Authority = false;
        Flag = true;
    }
}