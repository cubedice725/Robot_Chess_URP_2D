using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public abstract class Skill : MonoBehaviour
{
    public float angle = 0;
    public bool UpdateLookAtTarget(Vector3 target, float accuracy, float rotationSpeed)
    {
        // 타겟 방향 계산
        Vector3 direction = target - transform.position;
        direction.Normalize(); // 방향 벡터 정규화
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(angle)));

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
            angle += 180;
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
    public bool UpdateSelectionCheck()
    {
        if (Instance.hit != null && Instance.hit.name.StartsWith("Selection"))
        {
            return true;
        }
        return false;
    }
}
