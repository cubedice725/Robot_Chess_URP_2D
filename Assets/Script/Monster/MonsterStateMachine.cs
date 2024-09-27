using UnityEngine;

public class MonsterStateMachine : MonoBehaviour
{
    public IState CurrentState { get; private set; }

    public IState monsterSkillCastingState;
    public IState monsterMovingState;
    public IState monsterIdleState;

    public virtual void Awake() 
    {
        monsterIdleState = GetComponent<MonsterIdleState>();
        monsterSkillCastingState = GetComponent<MonsterSkillCastingState>();
        monsterMovingState = GetComponent<MonsterMovingState>();
    }
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }
    public void TransitionTo(IState nextState)
    {
        if (nextState == CurrentState)
            return;
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();

    }
    public void MonsterStateMachineUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.IStateUpdate();
        }
    }
}
