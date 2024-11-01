using UnityEngine;
using static GameManager;

public class PlayerIdleState : IState
{
    private PlayerMovement _playerMovement;

    public PlayerIdleState(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
    }
    public void Entry()
    {

    }
    public void IStateUpdate()
    {

    }
    public bool Exit()
    {
        return false;
    }
}
