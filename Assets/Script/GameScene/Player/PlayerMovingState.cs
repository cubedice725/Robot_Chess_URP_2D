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
        Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y] = (int)MapObject.noting;
        _playerMovement.MoveReady();
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
                Instance.playerState = State.Idle;
            }
        }

    }
    public bool Exit()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.MovePlane);
        if (moveUse)
        {
            Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y] = (int)MapObject.player;
            moveUse = false;
            Instance.player.MoveCount++;
            Instance.ButtonLock = false;
            return true;
        }
        Instance.ButtonLock = false;
        return false;
    }
}