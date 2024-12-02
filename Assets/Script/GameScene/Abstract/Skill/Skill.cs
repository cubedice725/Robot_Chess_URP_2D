using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public abstract class Skill : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public MyObject myObject;
    public virtual int UsageLimit { get; set; }
    // ���� ���Ƚ��
    public int Usage { get; set; } = 0;
    public void Awake()
    {
        playerMovement = GameManager.Instance.player.GetComponent<PlayerMovement>();
        myObject = GetComponent<MyObject>();
    }
    public bool UpdateLookAtTarget(Vector3 target, float accuracy, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(TargetToAngle(target))));

        // ���� ȸ�� ������ ��ǥ ���� ���� ���
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        // ���� ���̰� �Ӱ谪���� Ŭ ��쿡�� ȸ��
        if (angleDifference > accuracy)
        {
            // �ε巴�� ȸ��
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            return false;
        }
        return true;
    }
    // �����ϰ�� �ݴ�� ����
    public float LeftAbj(float angle)
    {
        // �����ϰ�� ��ü�� �ݴ�� ����
        if (Mathf.Sign(Instance.player.transform.localScale.x) < 0)
        {
            angle -= 180;
        }
        return angle;
    }
    public bool SkillArray(Vector3 position, float rotationSpeed)
    {
        if (transform.rotation != Quaternion.Euler(position))
        {
            transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(position),
            rotationSpeed * Time.deltaTime);
            return false;
        }
        return true;
    }

    // Ÿ�ٿ� ���� ���
    public float TargetToAngle(Vector3 target)
    {
        float angle;
        Vector3 direction = target - transform.position;
        direction.Normalize(); // ���� ���� ����ȭ
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
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
    public void AttackRange(int size, Vector3Int targetPos = new Vector3Int(), Vector2Int startPos = new Vector2Int(), Vector2Int endPos = new Vector2Int())
    {
        targetPos = Instance.PlayerPositionInt;
        startPos = Vector2Int.zero;
        endPos = new Vector2Int(Instance.MapSizeX, Instance.MapSizeY);

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
                if (Instance.Map2D[targetPos.x, downPos] == (int)MapObject.moster)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
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
                if (Instance.Map2D[targetPos.x, upPos] == (int)MapObject.moster)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
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
                if (Instance.Map2D[leftPos, targetPos.y] == (int)MapObject.moster)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
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
                if (Instance.Map2D[rightPos, targetPos.y] == (int)MapObject.moster)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
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
}
