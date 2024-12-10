using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Skill : MonoBehaviour
{
    public enum AttackType
    {
        Normal,
        Schrotflinte,
        LaserGun
    }

    public PlayerMovement playerMovement;
    public MyObject myObject;
    public virtual int UsageLimit { get; set; }
    // 무기 사용횟수
    public int Usage { get; set; } = 0;
    public void Awake()
    {
        playerMovement = GameManager.Instance.player.GetComponent<PlayerMovement>();
        myObject = GetComponent<MyObject>();
    }
    public bool UpdateSelectionCheck()
    {
        if (Instance.MyHit != null && Instance.MyHit.name.StartsWith("Selection"))
        {
            Instance.MyHit.name = "";
            Instance.ButtonLock = true;
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.DamagedArea);
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);
            playerMovement.LookMonsterAnimation(Instance.MyHit.positionInt.x);
            return true;
        }
        return false;
    }
    public void CheckUsage()
    {
        if(Usage >= UsageLimit)
        {
            transform.parent = null;
            if(myObject.transform.localScale.x < 0)
            {
                myObject.transform.localScale = new Vector3(-myObject.transform.localScale.x, myObject.transform.localScale.y, 0);
            }
            myObject.transform.rotation = Quaternion.Euler(Vector3Int.zero);
            Usage = 0;
            myObject.Destroy();
        }
    }
    public void UnifiedAttackRange(int size, AttackType attackType, Vector3Int targetPos = new Vector3Int(), Vector2Int startPos = new Vector2Int(), Vector2Int endPos = new Vector2Int())
    {
        targetPos = Instance.PlayerPositionInt;
        startPos = Vector2Int.zero;
        endPos = new Vector2Int(Instance.MapSizeX - 1, Instance.MapSizeY - 1);

        bool downSide = targetPos.y > startPos.y;
        bool upSide = targetPos.y < endPos.y;
        bool leftSide = targetPos.x > startPos.x;
        bool rightSide = targetPos.x < endPos.x;

        // Up, Down, Left, Right
        bool[] isDamagedArea = new bool[4] { true, true, true, true };
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

            switch (attackType)
            {
                case AttackType.Normal:
                    {
                        if (isDamagedArea[0])
                        {
                            isDamagedArea[0] = CheckAndPlaceSelection(upPosSide && leftSide && rightSide, targetPos.x, upPos, isDamagedArea[0]);
                        }
                        else
                        {
                            CheckAndPlaceSelection(upPosSide && leftSide && rightSide, targetPos.x, upPos, isDamagedArea[0]);
                        }
                        if (isDamagedArea[1])
                        {
                            isDamagedArea[1] = CheckAndPlaceSelection(downPosSide && leftSide && rightSide, targetPos.x, downPos, isDamagedArea[1]);
                        }
                        else
                        {
                            CheckAndPlaceSelection(downPosSide && leftSide && rightSide, targetPos.x, downPos, isDamagedArea[1]);
                        }
                        if (isDamagedArea[2])
                        {
                            isDamagedArea[2] = CheckAndPlaceSelection(leftPosSide && upSide && downSide, leftPos, targetPos.y, isDamagedArea[2]);
                        }
                        else
                        {
                            CheckAndPlaceSelection(leftPosSide && upSide && downSide, leftPos, targetPos.y, isDamagedArea[2]);
                        }
                        if (isDamagedArea[3])
                        {
                            isDamagedArea[3] = CheckAndPlaceSelection(rightPosSide && upSide && downSide, rightPos, targetPos.y, isDamagedArea[3]);
                        }
                        else
                        {
                            CheckAndPlaceSelection(rightPosSide && upSide && downSide, rightPos, targetPos.y, isDamagedArea[3]);
                        }
                        break;
                    }
                case AttackType.LaserGun:
                    {
                        CheckAndPlaceSelection(upPosSide && leftSide && rightSide, targetPos.x, upPos, index);
                        CheckAndPlaceSelection(downPosSide && leftSide && rightSide, targetPos.x, downPos, index);
                        CheckAndPlaceSelection(leftPosSide && upSide && downSide, leftPos, targetPos.y, index);
                        CheckAndPlaceSelection(rightPosSide && upSide && downSide, rightPos, targetPos.y, index);
                        break;
                    }
                case AttackType.Schrotflinte:
                    {
                        CheckAndPlaceSelection(upPosSide && leftSide && rightSide, targetPos.x, upPos, index);
                        CheckAndPlaceSelection(downPosSide && leftSide && rightSide, targetPos.x, downPos, index);
                        CheckAndPlaceSelection(leftPosSide && upSide && downSide, leftPos, targetPos.y, index);
                        CheckAndPlaceSelection(rightPosSide && upSide && downSide, rightPos, targetPos.y, index);

                        upPos = targetPos.y + index + 1;
                        upPosSide = upPos > startPos.y && upPos < endPos.y;
                        CheckAndPlaceSelection(leftPosSide && upSide && downSide, leftPos, targetPos.y + 1, true, false);

                        downPos = targetPos.y - index - 1;
                        downPosSide = downPos > startPos.y && downPos < endPos.y;
                        CheckAndPlaceSelection(leftPosSide && upSide && downSide, leftPos, targetPos.y - 1, true, false);


                        upPos = targetPos.y + index + 1;
                        upPosSide = upPos > startPos.y && upPos < endPos.y;
                        CheckAndPlaceSelection(rightPosSide && upSide && downSide, rightPos, targetPos.y + 1, true, false);

                        downPos = targetPos.y - index - 1;
                        downPosSide = downPos > startPos.y && downPos < endPos.y;
                        CheckAndPlaceSelection(rightPosSide && upSide && downSide, rightPos, targetPos.y - 1, true, false);

                        break;
                    }
                default:
                    break;
            }
        }
    }
    // 물체가 앞에 있어 막히는 경우 사용
    private bool CheckAndPlaceSelection(bool condition, int x, int y, bool isDamagedArea, bool isSelection = true)
    {
        if (condition)
        {
            if (Instance.Map2D[x, y] == (int)MapObject.moster)
            {
                if (isDamagedArea)
                {
                    PlaceSelection(x, y, PoolManager.Prefabs.DamagedArea);
                }
                if (isSelection)
                {
                    PlaceSelection(x, y, PoolManager.Prefabs.Selection);
                }
                return false;
            }
            else
            {
                PlaceSelection(x, y, PoolManager.Prefabs.UnSelection);
            }
        }
        return true;
    }
    // 직선 범위공격시
    private void CheckAndPlaceSelection(bool condition, int x, int y, int index)
    {
        if (condition)
        {
            if (index == 1)
            {
                if (Instance.Map2D[x, y] == (int)MapObject.moster)
                {
                    PlaceSelection(x, y, PoolManager.Prefabs.DamagedArea);
                }
                PlaceSelection(x, y, PoolManager.Prefabs.Selection);
            }
            else if(Instance.Map2D[x, y] == (int)MapObject.moster)
            {
                PlaceSelection(x, y, PoolManager.Prefabs.DamagedArea);
            }
            else
            {
                PlaceSelection(x, y, PoolManager.Prefabs.UnSelection);
            }
        }
    }
    
    private void PlaceSelection(int x, int y, PoolManager.Prefabs prefabType)
    {
        Instance.poolManager.SelectPool(prefabType).Get().transform.position = new Vector3(x, y, -1);
    }

    
}
