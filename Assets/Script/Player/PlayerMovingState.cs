using UnityEngine;
using static GameManager;

public class PlayerMovingState : IState
{
    private PlayerMovement _playerMovement;
    private RaycastHit hit;
    private bool moveUse  = false;  
    public PlayerMovingState(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
    }
    public void Entry()
    {
        _playerMovement.SetMovePlane();
    }
    public void IStateUpdate()
    {
        //플레이어 판을 클릭하면
        if (_playerMovement.UpdateMovePlaneCheck())
        {
            moveUse = true;
        }
        if (moveUse) 
        {
            if (!_playerMovement.UpdateMove())
            {
                _playerMovement.IdleState();
            }
        }

    }
    public bool Exit()
    {
        _playerMovement.RemoveMovePlane();
        if (moveUse)
        {
            moveUse = false;
            Instance.player.MoveCount++;
            return true;
        }
        return false;
    }
}