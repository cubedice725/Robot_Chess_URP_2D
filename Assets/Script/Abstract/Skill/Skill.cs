using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public abstract class Skill : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public MyObject myObject;
    public virtual int UsageLimit { get; set; }
    public int Usage { get; set; } = 0;
    public void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        myObject = GetComponent<MyObject>();
    }
    public bool UpdateLookAtTarget(Vector3 target, float accuracy, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(TargetToAngle(target))));

        // 현재 회전 각도와 목표 각도 차이 계산
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        // 각도 차이가 임계값보다 클 경우에만 회전
        if (angleDifference > accuracy)
        {
            // 부드럽게 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            return false;
        }
        return true;
    }
    // 좌측일경우 반대로 돌림
    public float LeftAbj(float angle)
    {
        // 좌측일경우 물체를 반대로 돌림
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

    // 타겟에 각도 계산
    public float TargetToAngle(Vector3 target)
    {
        float angle;
        Vector3 direction = target - transform.position;
        direction.Normalize(); // 방향 벡터 정규화
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

    public bool UpdateSelectionCheck()
    {
        if (Instance.hit != null && Instance.hit.name.StartsWith("Selection"))
        {
            Instance.ButtonLock = true;
            return true;
        }
        return false;
    }
    public void AttackRange(int size)
    {
        for (int i = 1; i <= size; i++)
        {
            if ((Instance.PlayerPositionInt.y - i > 0)  && Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y - i] == (int)MapObject.moster)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                    = new Vector2(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y - i);
            }
            if ((Instance.PlayerPositionInt.y + i < Instance.MapSizeY) && Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y + i] == (int)MapObject.moster)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position 
                    = new Vector2(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y + i);
            }
            if ((Instance.PlayerPositionInt.x - i > 0) && Instance.Map2D[Instance.PlayerPositionInt.x - i, Instance.PlayerPositionInt.y] == (int)MapObject.moster)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                    = new Vector2(Instance.PlayerPositionInt.x - i, Instance.PlayerPositionInt.y);
            }
            if ((Instance.PlayerPositionInt.x + i < Instance.MapSizeX) && Instance.Map2D[Instance.PlayerPositionInt.x + i, Instance.PlayerPositionInt.y] == (int)MapObject.moster)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                    = new Vector2(Instance.PlayerPositionInt.x + i, Instance.PlayerPositionInt.y);
            }
        }
    }
    public void CheckUsage()
    {
        if(Usage >= UsageLimit)
        {
            transform.parent = null;
            myObject.Destroy();
        }
    }
}
