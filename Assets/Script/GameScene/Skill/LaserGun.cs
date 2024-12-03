using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;
public class LaserGun : Skill, IState
{
    // .03 .15

    bool skillUse = false;
    bool start = false;
    bool ready = true;
    Vector3 targetPos;
    Vector3Int mousePos;
    GameObject laserPoint;
    Vector3Int laserPointPos;
    public override int UsageLimit { get => 1; set { } }

    public void Entry()
    {
        laserPoint = GameObject.Find("LaserPoint");
        LaserGunAttackRange(1);
    }
    public void IStateUpdate()
    {
        string direction = "";
        if (ready)
        {
            transform.position = new Vector3(
                Instance.player.transform.position.x + (Instance.player.transform.position.x - Instance.MousePos.x),
                Instance.player.transform.position.y + (Instance.player.transform.position.y - Instance.MousePos.y), 0);
            UpdateLookAtTarget(Instance.MousePos, 0.001f, 30f);
        }
        if (!start && UpdateSelectionCheck())
        {
            direction = DirectionCheck(Instance.MyHit.positionInt);
            if (direction == "Right")
            {
                targetPos = new Vector3(0.5f, 10.5f, 0);
            }
            if (direction == "Left")
            {
                targetPos = new Vector3(10.5f, 0.5f, 0);
            }
            if (direction == "Up")
            {
                targetPos = new Vector3(0.5f, 0.5f, 0);
            }
            if (direction == "Down")
            {
                targetPos = new Vector3(10.5f, 10.5f, 0);
            }
            ready = false;
            start = true;
        }
        if (start)
        {
            bool move = !UpdateSkillMove(transform, targetPos, 0.001f, 30f);
            bool look = !UpdateLookAtTarget(Instance.MyHit.positionInt, 0.001f, 30f);
            laserPoint.transform.position = Instance.MyHit.positionInt;
            if (move && look)
            {
                direction = DirectionCheck(Instance.MyHit.positionInt);
                if (direction == "Right")
                {
                    targetPos = new Vector3(Instance.MapSizeX - 2, Instance.MyHit.positionInt.y, 0);
                }
                if (direction == "Left")
                {
                    targetPos = new Vector3(1, Instance.MyHit.positionInt.y, 0);
                }
                if (direction == "Up")
                {
                    targetPos = new Vector3(Instance.MyHit.positionInt.x, Instance.MapSizeY - 2, 0);
                }
                if (direction == "Down")
                {
                    targetPos = new Vector3(Instance.MyHit.positionInt.x, 1, 0);
                }
                Usage++;
                skillUse = true;
                start = false;
            }
        }
        if (skillUse)
        {
            UpdateSkillMove(laserPoint.transform, targetPos, 0.001f, 10f);
            float line = Vector2.Distance(transform.position, laserPoint.transform.position);
            transform.GetChild(0).transform.localScale = new Vector3(line /2 + 0.5f, 0.15f, 0);
            UpdateLookAtTarget(laserPoint.transform.position, 0.001f, 10f);
            
            Vector3Int laserPointrPosInt = new Vector3Int(
                (int)Mathf.Round(laserPoint.transform.position.x),
                (int)Mathf.Round(laserPoint.transform.position.y),
                (int)Mathf.Round(transform.position.z)
            );
            if (laserPointPos != laserPointrPosInt)
            {
                laserPointPos = laserPointrPosInt;
                if (Instance.Map2D[laserPointrPosInt.x, laserPointrPosInt.y] == (int)MapObject.moster)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                        = laserPointPos;
                }
            }
            if (Vector2.Distance(laserPoint.transform.position, targetPos) < 0.001f)
            {
                transform.GetChild(0).transform.localScale = new Vector3(0, 0, 0);
                Instance.playerState = State.Idle;
                ready = true;
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
    public void LaserGunAttackRange(int size, Vector3Int targetPos = new Vector3Int(), Vector2Int startPos = new Vector2Int(), Vector2Int endPos = new Vector2Int())
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

    public string DirectionCheck(Vector3Int mousePosInt)
    {
        Vector3 direction = mousePosInt - Instance.PlayerPositionInt;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                return "Right";
            }
            else
            {
                return "Left";
            }
        }
        else
        {
            if (direction.y > 0)
            {
                return "Up";
            }
            else
            {
                return "Down";
            }
        }
    }
}
