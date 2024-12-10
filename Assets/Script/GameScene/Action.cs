using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Action : MonoBehaviour
{
    public bool UpdateLookAtTarget(Vector3 target, Transform transObject, float accuracy, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(transObject, TargetToAngle(transObject, target))));

        // ���� ȸ�� ������ ��ǥ ���� ���� ���
        float angleDifference = Quaternion.Angle(transObject.rotation, targetRotation);
        // ���� ���̰� �Ӱ谪���� Ŭ ��쿡�� ȸ��
        if (angleDifference > accuracy)
        {
            // �ε巴�� ȸ��
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
        // ���������� �����̴� ��ǥ�� �Ÿ�, ��ǥ ������ ���⼭ ����
        transObject.position = Vector2.MoveTowards(transObject.position, targetPos, speed * Time.deltaTime);
        if (distance > accuracy)
        {
            return true;
        }
        return false;
    }
    // �����ϰ�� �ݴ�� ����
    public float LeftAbj(Transform transObject, float angle)
    {
        // �����ϰ�� ��ü�� �ݴ�� ����
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

    // Ÿ�ٿ� ���� ���
    public float TargetToAngle(Transform transObject, Vector3 target)
    {
        float angle;
        Vector3 direction = target - transObject.position;
        direction.Normalize(); // ���� ���� ����ȭ
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
}
