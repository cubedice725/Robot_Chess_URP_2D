using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    // ������ �ൿ�� ���� ��� True�� �ٲ����
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
        //���� �ϰ� ������� ������
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
        // ���� ���� ��� �����ڰ� �ۼ��Ͽ� ���� �������� ����
        UpdateMonster();

        //���� ���� �ƴѰ�� ������
        if (!GameManager.Instance.monsterTurn)
        {
            monsterState = MonsterState.Idle;
        }
    }
    public abstract void UpdateMonster();
}