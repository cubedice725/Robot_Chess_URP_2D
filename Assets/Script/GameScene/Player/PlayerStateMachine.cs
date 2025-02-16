using Unity.VisualScripting.FullSerializer;
using static GameManager;
public class PlayerStateMachine
{
    public IState CurrentState {  get; private set; }

    public PlayerSkillCastingState playerSkillCastingState;
    public PlayerMovingState playerMovingState;
    public PlayerIdleState playerIdleState;
    
    public PlayerStateMachine(PlayerMovement playerMovement)
    {
        playerSkillCastingState = new PlayerSkillCastingState();
        playerMovingState = new PlayerMovingState(playerMovement);
        playerIdleState = new PlayerIdleState(playerMovement);
    }
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Entry();
    }
    public void TransitionTo(IState nextState)
    {
        if (nextState == CurrentState) return;

        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Entry();
    }
    public void PlayerStateMachineUpdate()
    {
        if (CurrentState != null) 
        {
            CurrentState.IStateUpdate();
        }
    }
}
