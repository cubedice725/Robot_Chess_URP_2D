using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class MoveDistanceUp : Skill, IState
{
    public override int UsageLimit { get => 2; set { } }
    private bool skillUse = false;
    public void Entry()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.MovePlane);

        Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y] = (int)MapObject.noting;
        Instance.player.MoveDistance = 2;
        playerMovement.MoveReady();
    }
    public void IStateUpdate()
    {

        //�÷��̾� ���� Ŭ���ϸ�
        if (playerMovement.UpdateMovePlaneCheck())
        {
            skillUse = true;
        }
        if (skillUse)
        {
            if (!playerMovement.UpdateMove())
            {
                Instance.playerState = State.Idle;
            }
        }
    }
    public bool Exit()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.MovePlane);
        Instance.player.MoveDistance = 1;

        if (skillUse)
        {
            Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y] = (int)MapObject.player;
            skillUse = false;
            Instance.player.MoveCount++;
            Instance.ButtonLock = false;
            return true;
        }
        Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y] = (int)MapObject.player;
        return false;
    }
}
