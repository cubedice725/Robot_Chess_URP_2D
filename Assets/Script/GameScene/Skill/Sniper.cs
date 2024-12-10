using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Sniper : LongSkill, IState
{
    Vector3Int target;
    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;
    public override int UsageLimit { get => 2; set { } }
    public void Entry()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);
        UnifiedAttackRange(3, AttackType.Normal);
    }
    public void IStateUpdate()
    {
        if (UpdateSelectionCheck())
        {
            skillUse = true;
            start = true;
        }

        if (start)
        {
            if (!Instance.action.UpdateLookAtTarget(Instance.MyHit.positionInt, transform, accuracy, 7f))
            {
                Shoot(PoolManager.Prefabs.AK47Bullet);
                start = false;
                Usage++;
            }
        }
        else if (skillUse)
        {
            if (Instance.action.TurnAngle(Vector3.zero, transform,7f))
            {
                Instance.playerState = State.Idle;
                CheckUsage();
            }
        }
    }
    public bool Exit()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.DamagedArea);

        if (skillUse)
        {
            skillUse = false;
            return true;
        }
        return false;
    }
}
