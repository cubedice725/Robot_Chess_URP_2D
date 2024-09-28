using UnityEngine;

public class PlayerMovingState : IState
{
    private PlayerMovement _playerMovement;
    private RaycastHit hit;
    private bool Moveing  = false;  
    public PlayerMovingState(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
    }
    public void Entry()
    {
        _playerMovement.SetPlayerPlane();
    }
    public void IStateUpdate()
    {
        if (_playerMovement.UpdatePlayerMovePlaneCheck())
        {
            Moveing = true;
        }
        if (Moveing) 
        {
            if (!_playerMovement.UpdateMove())
            {
                Moveing = false;
            }
        }

    }
    public void Exit()
    {
        _playerMovement.RemovePlayerPlane();
    }
}