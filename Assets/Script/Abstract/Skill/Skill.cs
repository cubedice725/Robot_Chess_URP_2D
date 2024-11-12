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
