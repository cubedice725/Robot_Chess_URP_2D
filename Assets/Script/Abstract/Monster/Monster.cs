using UnityEngine;
using static GameManager;


// 에러 발생시 고려해야할 사항
// Authority을 행동에 할당했는지
public abstract class Monster : MonoBehaviour
{
    // 몬스터의 행동이 끝날 경우 True로 바꿔야함
    public bool Flag { get; set; } = false;

    // 몬스터가 이동시 겹치는 것을 방지하기 위해 만듦
    // 어떠한 행동을 하기 위해서는 Authority에 true가 몬스터에게 있어야함
    // 즉 권한을 가지면 몬스터가 움직여도 된다는 의미(스킬도 Authority를 가져야함)
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
        // 게임 시스템은 Authority을 통해 Flag로 간다.
        // 즉 Authority가 True이면 무조건 False가 된후에 Flag가 True가 된다.
        // Authority, Flag가 둘다 True가 되는경우는 없다.
        // Die && Authority && Flag의 경우를 만드는 곳은 GameManager이다.
        if (Die && Authority && Flag) return;

        // 어떠한 경우에 상태 변환이 될지 모르기에 상태 변환을 가장 먼저 해야함
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
        // 사망판정
        if (HP <= 0 && !Die)
        {
            // 가만히 있으면 해당 자리에 몬스터가 맵에 남아있는걸 방지하기 위해 만듦
            if (state == State.Idle) 
            {
                Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.noting;
            }
            boxCollider2D.enabled = false;
            rigi2D.simulated = false;

            Instance.GameScore += ScorePoint;
            // 사망 애니메이션은 반복적으로 true를 받으면 멈춤 그렇기에 사망은 한번만해야함
            Die = true;
            // 사망 애니메이션과 나중에 갈 목적지에 있는 몬스터를 맵에서 지움
            monsterMovement.Die();
            GetComponent<SpriteRenderer>().sortingOrder = 1;

        }

        // 사망 이후 Authority이 들어오면 이를 넘기기위함
        if (Die && Authority)
        {
            TurnPass();
        }

        // 몬스터의 상태변환을 만들기 위함
        UpdateMonster();
        // 몬스터의 상태 클래스에 연결되어있음
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