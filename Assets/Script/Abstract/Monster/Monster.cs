using AnimationImporter.PyxelEdit;
using UnityEngine;

public class Monster : MonoBehaviour
{
    // 몬스터의 행동이 끝날 경우 True로 바꿔야함
    public bool flag = false;
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
        if (HP <= 0 && !Die)
        {
            Die = true;
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
            flag = true;
            return;
        }
        // 몬스터 턴인 경우 개발자가 작성하여 몬스터 움직임을 설정
        UpdateMonster();

        monsterStateMachine.MonsterStateMachineUpdate();
    }
    public virtual void UpdateMonster() { }
}