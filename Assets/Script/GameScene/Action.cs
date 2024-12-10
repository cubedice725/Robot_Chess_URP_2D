using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Action : MonoBehaviour
{
    public bool UpdateLookAtTarget(Vector3 target, Transform transObject, float accuracy, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(transObject, TargetToAngle(transObject, target))));

        // 현재 회전 각도와 목표 각도 차이 계산
        float angleDifference = Quaternion.Angle(transObject.rotation, targetRotation);
        // 각도 차이가 임계값보다 클 경우에만 회전
        if (angleDifference > accuracy)
        {
            // 부드럽게 회전
            transObject.rotation = Quaternion.Slerp(transObject.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            return false;
        }
        return true;
    }
    public bool UpdateMove(Transform transObject, Vector3 targetPos, float accuracy, float speed)
    {
        float distance = Vector2.Distance(transObject.position, targetPos);
        // 최종적으로 움직이는 좌표의 거리, 좌표 오차는 여기서 수정
        transObject.position = Vector2.MoveTowards(transObject.position, targetPos, speed * Time.deltaTime);
        if (distance > accuracy)
        {
            return true;
        }
        return false;
    }
    // 좌측일경우 반대로 돌림
    public float LeftAbj(Transform transObject, float angle)
    {
        // 좌측일경우 물체를 반대로 돌림
        if (Mathf.Sign(transObject.root.localScale.x) < 0)
        {
            angle -= 180;
        }
        return angle;
    }
    public bool TurnAngle(Vector3 position, Transform transObject, float rotationSpeed)
    {
        if (transObject.rotation != Quaternion.Euler(position))
        {
            transObject.rotation = Quaternion.Slerp(
            transObject.rotation,
            Quaternion.Euler(position),
            rotationSpeed * Time.deltaTime);
            return false;
        }
        return true;
    }

    // 타겟에 각도 계산
    public float TargetToAngle(Transform transObject, Vector3 target)
    {
        float angle;
        Vector3 direction = target - transObject.position;
        direction.Normalize(); // 방향 벡터 정규화
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
}
