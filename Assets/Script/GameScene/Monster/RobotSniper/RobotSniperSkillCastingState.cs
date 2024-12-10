using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class RobotSniperSkillCastingState : MonoBehaviour, IState
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
        if (start && !Instance.action.UpdateLookAtTarget(new Vector3Int(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y, 0), transform.GetChild(0), 0.001f, 7f))
        {
            Shoot(PoolManager.Prefabs.AK47Bullet);
            start = false;
        }
        else if (!start && Instance.action.TurnAngle(Vector3.zero, transform.GetChild(0), 7f))
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
        ThrowableObject.GetComponent<AK47Bullet>().isMonster = true;
        ThrowableObject.gameObject.SetActive(false);
        ThrowableObject.transform.position = transform.GetChild(0).transform.GetChild(0).transform.position;
        ThrowableObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Instance.action.LeftAbj(transform, transform.GetChild(0).transform.GetChild(0).eulerAngles.z)));
        ThrowableObject.gameObject.SetActive(true);
    }
}
