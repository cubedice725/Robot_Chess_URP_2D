using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public abstract class Skill : MonoBehaviour
{
    public float angle = 0;
    public bool UpdateLookAtTarget(Vector3 target, float accuracy, float rotationSpeed)
    {
        // Ÿ�� ���� ���
        Vector3 direction = target - transform.position;
        direction.Normalize(); // ���� ���� ����ȭ
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(angle)));

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
