using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
public class RobotKnifeSkillCastingState : MonoBehaviour, IState
{
    MonsterMovement monsterMovement;
    bool start = true;
    public void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
    }
    public void Entry()
    {
        monsterMovement.Authority(false);
    }
    public void IStateUpdate()
    {
        if (start && !UpdateLookAtTarget(new Vector3Int(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y,0), 0.001f, 7f))
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position = Instance.PlayerPositionInt;
            start = false;
        }
        else if (!start && SkillArray(new Vector3(0, 0, LeftAbj(90)), 7f))
        {
            monsterMovement.IdleState();
        }
    }
    public bool Exit()
    {
        start = true;
        return true;
    }
    public bool UpdateLookAtTarget(Vector3Int target, float accuracy, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(TargetToAngle(target))));

        // ���� ȸ�� ������ ��ǥ ���� ���� ���
        float angleDifference = Quaternion.Angle(transform.GetChild(0).rotation, targetRotation);
        // ���� ���̰� �Ӱ谪���� Ŭ ��쿡�� ȸ��
        if (angleDifference > accuracy)
        {
            // �ε巴�� ȸ��
            transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        if (Mathf.Sign(transform.localScale.x) < 0)
        {
            angle -= 180;
        }
        return angle;
    }
    // Ÿ�ٿ� ���� ���
    public float TargetToAngle(Vector3Int target)
    {
        float angle;
        Vector3 direction = target - transform.GetChild(0).position;
        direction.Normalize(); // ���� ���� ����ȭ
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
    public bool SkillArray(Vector3 position, float rotationSpeed)
    {
        if (transform.GetChild(0).rotation != Quaternion.Euler(position))
        {
            transform.GetChild(0).rotation = Quaternion.Slerp(
            transform.GetChild(0).rotation,
            Quaternion.Euler(position),
            rotationSpeed * Time.deltaTime);
            return false;
        }
        return true;
    }

}
