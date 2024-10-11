using UnityEngine;
using static GameManager;


// 에러 발생시 고려해야할 사항
// Authority을 행동에 할당했는지
public class Monster : MonoBehaviour
{
    // 몬스터의 행동이 끝날 경우 True로 바꿔야함
    public bool Flag { get; set; } = false;

    // 몬스터가 이동시 겹치는 것을 방지하기 위해 만듦
    // 어떠한 행동을 하기 위해서는 Authority에 true가 몬스터에게 있어야함
    // 즉 권한을 가지면 몬스터가 움직여도 된다는 의미(스킬도 Authority를 가져야함)
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
        // 사망판정
        if (HP <= 0 && !Die)
        {
            // 가만히 있으면 해당 자리에 몬스터가 맵에 남아있는걸 방지하기 위해 만듦
            if (state == State.Idle) 
            {
                Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.noting;
            }
            // 사망 애니메이션은 반복적으로 true를 받으면 멈춤 그렇기에 사망은 한번만해야함
            Die = true;
            // 사망 애니메이션과 나중에 갈 목적지에 있는 몬스터를 맵에서 지움
            monsterMovement.Die();
        }
        //몬스터 턴과 상관없이 움직임
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
            // 사망하면 spawnMonsters에 남아있기 때문에 권한이랑 플래그는 항상 다음으로 넘길수 있도록함
            Authority = false;
            Flag = true;
            return;
        }
        // 몬스터 턴인 경우 개발자가 작성하여 몬스터 움직임을 설정
        UpdateMonster();

        monsterStateMachine.MonsterStateMachineUpdate();
    }
    public virtual void UpdateMonster() { }
}