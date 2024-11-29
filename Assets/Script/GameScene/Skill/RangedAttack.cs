using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class RangedAttack : MonoBehaviour, IState
{
    Vector3Int CurrentMousPos = Vector3Int.zero;
    public void Entry()
    {
        Instance.ButtonLock = true;
    }

    public void IStateUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mousePosInt = new Vector3Int(
            (int)Mathf.Round(mousePos.x),
            (int)Mathf.Round(mousePos.y),
            -1
        );

        if (CurrentMousPos == mousePosInt) return;

        CurrentMousPos = mousePosInt;
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.DamagedArea);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);

        AttackRange(1, CurrentMousPos, Vector2Int.zero, new Vector2Int(Instance.MapSizeX - 1, Instance.MapSizeY - 1));
    }

    public bool Exit()
    {
        Instance.ButtonLock = false;

        return true;
    }
    public void AttackRange(int size, Vector3Int targetPos, Vector2Int startPos, Vector2Int endPos)
    {
        bool downSide = targetPos.y > startPos.y;
        bool upSide = targetPos.y < endPos.y;
        bool leftSide = targetPos.x > startPos.x;
        bool rightSide = targetPos.x < endPos.x;

        if (downSide && upSide && rightSide && leftSide)
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                = new Vector3(targetPos.x, targetPos.y, -1);
        }
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
                if (Instance.Map2D[targetPos.x, downPos] == (int)MapObject.moster || Instance.Map2D[targetPos.x, downPos] == (int)MapObject.player)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                        = new Vector3(targetPos.x, downPos, -1);
                }
                else
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.UnSelection).Get().transform.position
                        = new Vector3(targetPos.x, downPos, -1);
                }
            }

            if (upPosSide && leftSide && rightSide)
            {
                if (Instance.Map2D[targetPos.x, upPos] == (int)MapObject.moster || Instance.Map2D[targetPos.x, upPos] == (int)MapObject.player)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                        = new Vector3(targetPos.x, upPos, -1);
                }
                else
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.UnSelection).Get().transform.position
                        = new Vector3(targetPos.x, upPos, -1);
                }
            }

            if (leftPosSide && upSide && downSide)
            {
                if (Instance.Map2D[leftPos, targetPos.y] == (int)MapObject.moster || Instance.Map2D[leftPos, targetPos.y] == (int)MapObject.player)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                        = new Vector3(leftPos, targetPos.y, -1);
                }
                else
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.UnSelection).Get().transform.position
                        = new Vector3(targetPos.x - index, targetPos.y, -1);
                }
            }

            if (rightPosSide && upSide && downSide)
            {
                if (Instance.Map2D[rightPos, targetPos.y] == (int)MapObject.moster || Instance.Map2D[rightPos, targetPos.y] == (int)MapObject.player)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                        = new Vector3(rightPos, targetPos.y, -1);
                }
                else
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.UnSelection).Get().transform.position
                        = new Vector3(rightPos, targetPos.y, -1);
                }
            }
        }
    }

    
}
