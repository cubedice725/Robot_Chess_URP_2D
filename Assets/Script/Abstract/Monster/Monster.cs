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
    public MonsterState monsterState = MonsterState.Idle;
    public MonsterStateMachine monsterStateMachine;
    public MonsterMovement monsterMovement;
    public virtual void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
        monsterStateMachine = GetComponent<MonsterStateMachine>();
        monsterStateMachine.Initialize(monsterStateMachine.monsterIdleState);
    }
    public void Update()
    {
        //���� �ϰ� ������� ������
        if (monsterState == MonsterState.Idle)
        {
            monsterStateMachine.TransitionTo(monsterStateMachine.monsterIdleState);
        }
        else if (monsterState == MonsterState.Move)
        {
            monsterStateMachine.TransitionTo(monsterStateMachine.monsterMovingState);
        }
        else if (monsterState == MonsterState.Skill)
        {
            monsterStateMachine.TransitionTo(monsterStateMachine.monsterSkillCastingState);
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