using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public abstract class LongSkill : Skill
{
    // UpdateLookAtTarget으로 방향을 돌려야 정확한 방향으로 감
    public void Shoot(PoolManager.Prefabs prefabs)
    {
        MyObject ThrowableObject = Instance.poolManager.SelectPool(prefabs).Get();
        ThrowableObject.transform.position = transform.GetChild(0).transform.position;
        ThrowableObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, LeftAbj(transform.eulerAngles.z)));
    }
}
