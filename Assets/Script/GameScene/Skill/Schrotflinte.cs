using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Schrotflinte : Skill, IState
{
    Vector3Int target;
    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;
    bool mousePosUp, mousePosDown, mousePosLeft, mousePosRight, mousePosMiddle;
    Vector3 mousePos = Vector3.zero;

    public override int UsageLimit { get => 4; set { } }

    public void Entry()
    {
        AttackRange(2);
    }
    public void IStateUpdate()
    {
        if (UpdateSelectionCheck())
        {
            start = true;
        }

        if (start)
        {
            if (!UpdateLookAtTarget(Instance.MyHit.positionInt, accuracy, 7f))
            {
                SchrotShoot(Instance.MouseDownPosInt);
                skillUse = true;
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
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);

        if (skillUse)
        {
            skillUse = false;
            return true;
        }
        return false;
    }
    public void SchrotShoot(Vector3Int mousePosInt)
    {
        Vector3 direction = mousePosInt - Instance.PlayerPositionInt;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (Instance.Map2D[mousePosInt.x, mousePosInt.y + 1] == (int)MapObject.moster || Instance.Map2D[mousePosInt.x, mousePosInt.y + 1] == (int)MapObject.player)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x, mousePosInt.y + 1, 0);
            }
            if (Instance.Map2D[mousePosInt.x, mousePosInt.y] == (int)MapObject.moster || Instance.Map2D[mousePosInt.x, mousePosInt.y] == (int)MapObject.player)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x, mousePosInt.y, 0);
            }
            if (Instance.Map2D[mousePosInt.x, mousePosInt.y - 1] == (int)MapObject.moster || Instance.Map2D[mousePosInt.x, mousePosInt.y - 1] == (int)MapObject.player)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x, mousePosInt.y - 1, 0);
            }
            for (int index = 0; index < 10; index++)
            {
                SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f) - 1, 0));
                SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
                SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f) + 1, 0));
            }
            
        }
        else
        {
            if (Instance.Map2D[mousePosInt.x + 1, mousePosInt.y] == (int)MapObject.moster || Instance.Map2D[mousePosInt.x + 1, mousePosInt.y] == (int)MapObject.player)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x + 1, mousePosInt.y + 1, 0);
            }
            if (Instance.Map2D[mousePosInt.x, mousePosInt.y] == (int)MapObject.moster || Instance.Map2D[mousePosInt.x, mousePosInt.y] == (int)MapObject.player)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x, mousePosInt.y, 0);
            }
            if (Instance.Map2D[mousePosInt.x - 1, mousePosInt.y] == (int)MapObject.moster || Instance.Map2D[mousePosInt.x - 1, mousePosInt.y] == (int)MapObject.player)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x - 1, mousePosInt.y, 0);
            }
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x + 1, mousePosInt.y, 0);
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x, mousePosInt.y, 0);
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x - 1, mousePosInt.y, 0);
            for (int index = 0; index < 10; index++)
            {
                SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f) + 1, mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
                SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
                SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f) - 1, mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
            }
        }
        
    }
    public void SpawnFakeBullet(Vector3 targetPos)
    {
        MyObject fakeBullet;
        fakeBullet = Instance.poolManager.SelectPool(PoolManager.Prefabs.FakeBullet).Get();
        fakeBullet.GetComponent<FakeBullet>().targetPos = targetPos;
        fakeBullet.transform.position = transform.GetChild(0).transform.position;
        fakeBullet.transform.rotation = Quaternion.Euler(new Vector3(
            0, 0, TargetToAngle(targetPos)));
    }
}
