public class MonsterStateMachine
{
    public Monster _monster;
    public IState CurrentState { get; private set; }
    public MonsterStateMachine(Monster monster)
    {
        _monster = monster;
    }
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Entry();
    }
    public void TransitionTo(IState nextState)
    {
        if (nextState == CurrentState)
            return;
        if (CurrentState.Exit())
        {
            _monster.Flag = true;
        }
        CurrentState = nextState;
        nextState.Entry();

    }
    public void MonsterStateMachineUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.IStateUpdate();
        }
    }
}
