using UnityEngine;

public class PlayerIdleState : IState
{
    private PlayerMovement _playerMovement;

    public PlayerIdleState(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
    }
    public void Enter()
    {

    }
    public void IStateUpdate()
    {
        _playerMovement.UpdatePlayerCheck();
    }
    public void Exit()
    {
    }
}
