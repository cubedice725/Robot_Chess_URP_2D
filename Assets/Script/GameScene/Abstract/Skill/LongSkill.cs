using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public abstract class LongSkill : Skill
{
    // UpdateLookAtTarget���� ������ ������ ��Ȯ�� �������� ��
    public void Shoot(PoolManager.Prefabs prefabs)
    {
        MyObject ThrowableObject = Instance.poolManager.SelectPool(prefabs).Get();
        ThrowableObject.GetComponent<AK47Bullet>().isMonster = false;
        ThrowableObject.gameObject.SetActive(false);
        ThrowableObject.transform.position = transform.GetChild(0).transform.position;
        ThrowableObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Instance.action.LeftAbj(transform, transform.eulerAngles.z)));
        ThrowableObject.gameObject.SetActive(true);

    }
}
