using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    // 몬스터의 행동이 끝날 경우 True로 바꿔야함
    public bool flag = false;
    public virtual int AttackDistance { get; set; } = 1;
    public virtual int MovingDistance { get; set; } = 1;
    public enum MonsterState
    {
        Idle,
        Move,
        Skill
    }
    public IState monsterSkillCastingState;
    public IState monsterMovingState;
    public IState monsterIdleState;

    public MonsterState monsterState = MonsterState.Idle;
    public MonsterStateMachine monsterStateMachine;
    public MonsterMovement monsterMovement;
    public virtual void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
        monsterStateMachine = new MonsterStateMachine();
        monsterStateMachine.Initialize(monsterIdleState);
    }
    public void Update()
    {
        //몬스터 턴과 상관없이 움직임
        if (monsterState == MonsterState.Idle)
        {
            monsterStateMachine.TransitionTo(monsterIdleState);
        }
        else if (monsterState == MonsterState.Move)
        {
            monsterStateMachine.TransitionTo(monsterMovingState);
        }
        else if (monsterState == MonsterState.Skill)
        {
            monsterStateMachine.TransitionTo(monsterSkillCastingState);
        }
        monsterStateMachine.MonsterStateMachineUpdate();
        // 몬스터 턴인 경우 개발자가 작성하여 몬스터 움직임을 설정
        UpdateMonster();

        //몬스터 턴이 아닌경우 움직임
        if (!GameManager.Instance.monsterTurn)
        {
            monsterState = MonsterState.Idle;
        }
    }
    public abstract void UpdateMonster();
}