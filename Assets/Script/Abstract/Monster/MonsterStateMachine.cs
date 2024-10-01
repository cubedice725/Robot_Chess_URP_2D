public class MonsterStateMachine
{
    public IState CurrentState { get; private set; }

    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Entry();
    }
    public void TransitionTo(IState nextState)
    {
        if (nextState == CurrentState)
            return;
        CurrentState.Exit();    
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
