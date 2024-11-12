using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
public class Pistol : LongSkill, IState
{
    Vector3Int target;
    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;
    public override int UsageLimit { get; set; } = 6;

    public void Entry()
    {
        AttackRange(2);
    }
    public void IStateUpdate()
    {
        if (UpdateSelectionCheck())
        {
            Instance.MyObjectActivate = true;
            Instance.hit.name = "";
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
            playerMovement.LookMonsterAnimation(Instance.hit.positionInt.x);
            skillUse = true;
            start = true;
        }

        if (start)
        {
            if (!UpdateLookAtTarget(Instance.hit.positionInt, accuracy, 7f))
            {
                Shoot(PoolManager.Prefabs.AK47Bullet);
                start = false;
                Usage++;
            }
        }
        else if (skillUse)
        {
            if (SkillArray(Vector3.zero, 7f))
            {
                Instance.playerState = State.Idle;
                CheckUsage();
            }
        }
    }
    public bool Exit()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        if (skillUse)
        {
            skillUse = false;
            return true;
        }
        return false;
    }
}
