using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class RobotPistolSkillCastingState : MonoBehaviour, IState
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
        if (start && !UpdateLookAtTarget(new Vector3Int(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y, 0), 0.001f, 7f))
        {
            Shoot(PoolManager.Prefabs.AK47Bullet);
            start = false;
        }
        else if (!start && SkillArray(Vector3.zero, 7f))
        {
            monsterMovement.IdleState();
        }
    }
    public bool Exit()
    {
        start = true;
        return true;
    }
    public void Shoot(PoolManager.Prefabs prefabs)
    {
        MyObject ThrowableObject = Instance.poolManager.SelectPool(prefabs).Get();
        ThrowableObject.gameObject.SetActive(false);
        ThrowableObject.transform.position = transform.GetChild(0).transform.GetChild(0).transform.position;
        ThrowableObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(transform.GetChild(0).transform.GetChild(0).eulerAngles.z)));
        ThrowableObject.gameObject.SetActive(true);
    }
// 좌측일경우 반대로 돌림
    public float LeftAbj(float angle)
    {
        // 좌측일경우 물체를 반대로 돌림
        if (Mathf.Sign(transform.localScale.x) < 0)
        {
            angle -= 180;
        }
        return angle;
    }
    public bool UpdateLookAtTarget(Vector3Int target, float accuracy, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(TargetToAngle(target))));

        // 현재 회전 각도와 목표 각도 차이 계산
        float angleDifference = Quaternion.Angle(transform.GetChild(0).rotation, targetRotation);
        // 각도 차이가 임계값보다 클 경우에만 회전
        if (angleDifference > accuracy)
        {
            // 부드럽게 회전
            transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            return false;
        }
        return true;
    }
    // 타겟에 각도 계산
    public float TargetToAngle(Vector3Int target)
    {
        float angle;
        Vector3 direction = target - transform.GetChild(0).position;
        direction.Normalize(); // 방향 벡터 정규화
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
