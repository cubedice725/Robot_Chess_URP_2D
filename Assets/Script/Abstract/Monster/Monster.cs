using UnityEngine;

public class Monster : MonoBehaviour
{
    // 몬스터의 행동이 끝날 경우 True로 바꿔야함
    public bool flag = false;
    public int Num { get; set; }
    public virtual int AttackDistance { get; set; } = 1;
    public virtual int MovingDistance { get; set; } = 1;
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
        // 몬스터 턴인 경우 개발자가 작성하여 몬스터 움직임을 설정
        UpdateMonster();

        monsterStateMachine.MonsterStateMachineUpdate();
        
    }
    public virtual void UpdateMonster() { }
}