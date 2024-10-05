using UnityEngine;

public class Monster : MonoBehaviour
{
    // ������ �ൿ�� ���� ��� True�� �ٲ����
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
        // ���� ���� ��� �����ڰ� �ۼ��Ͽ� ���� �������� ����
        UpdateMonster();

        monsterStateMachine.MonsterStateMachineUpdate();
        
    }
    public virtual void UpdateMonster() { }
}