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

    public override int UsageLimit { get => 3; set { } }

    public void Entry()
    {
        SchrotflinteAttackRange(1);
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
                = new Vector3Int(mousePosInt.x + 1, mousePosInt.y, 0);
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
    public void SchrotflinteAttackRange(int size, Vector3Int targetPos = new Vector3Int(), Vector2Int startPos = new Vector2Int(), Vector2Int endPos = new Vector2Int())
    {
        targetPos = Instance.PlayerPositionInt;
        startPos = Vector2Int.zero;
        endPos = new Vector2Int(Instance.MapSizeX - 1, Instance.MapSizeY - 1);

        bool downSide = targetPos.y > startPos.y;
        bool upSide = targetPos.y < endPos.y;
        bool leftSide = targetPos.x > startPos.x;
        bool rightSide = targetPos.x < endPos.x;

        for (int index = 1; index <= size; index++)
        {
            int downPos = targetPos.y - index;
            int upPos = targetPos.y + index;
            int leftPos = targetPos.x - index;
            int rightPos = targetPos.x + index;

            bool downPosSide = downPos > startPos.y && downPos < endPos.y;
            bool upPosSide = upPos > startPos.y && upPos < endPos.y;
            bool leftPosSide = leftPos > startPos.x && leftPos < endPos.x;
            bool rightPosSide = rightPos > startPos.x && rightPos < endPos.x;


            if (downPosSide && leftSide && rightSide)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                    = new Vector3(targetPos.x, downPos, -1);
            }

            if (upPosSide && leftSide && rightSide)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                    = new Vector3(targetPos.x, upPos, -1);
            }

            if (leftPosSide && upSide && downSide)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                    = new Vector3(leftPos, targetPos.y, -1);
            }

            if (rightPosSide && upSide && downSide)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                    = new Vector3(rightPos, targetPos.y, -1);
            }
        }
    }
}
