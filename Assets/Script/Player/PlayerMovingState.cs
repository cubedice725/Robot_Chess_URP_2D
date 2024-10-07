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
        _playerMovement.SetMovePlane();
    }
    public void IStateUpdate()
    {
        //�÷��̾� ���� Ŭ���ϸ�
        if (_playerMovement.UpdateMovePlaneCheck())
        {
            Moveing = true;
        }
        if (Moveing) 
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
        if (Moveing)
        {
            Moveing = false;
            return true;
        }
        return false;
    }
}