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
        //�÷��̾� ���� Ŭ���ϸ�
        if (_playerMovement.UpdatePlayerMovePlaneCheck())
        {
            Moveing = true;
        }
        if (Moveing) 
        {
            if (!_playerMovement.UpdateMove())
            {
                Instance.playerState = PlayerState.Idle;
                Moveing = false;
            }
        }

    }
    public void Exit()
    {
        _playerMovement.RemovePlayerMovePlane();
    }
}