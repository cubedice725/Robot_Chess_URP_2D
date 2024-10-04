using UnityEngine;
using static GameManager;

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
        _playerMovement.SetPlayerMovePlane();
    }
    public void IStateUpdate()
    {
        //플레이어 판을 클릭하면
        if (_playerMovement.UpdatePlayerMovePlaneCheck())
        {
            Moveing = true;
        }
        if (Moveing) 
        {
            if (!_playerMovement.UpdateMove())
            {
                Instance.playerState = State.Idle;
            }
        }

    }
    public bool Exit()
    {
        _playerMovement.RemovePlayerMovePlane();
        if (Moveing)
        {
            Moveing = false;
            return true;
        }
        return false;
    }
}