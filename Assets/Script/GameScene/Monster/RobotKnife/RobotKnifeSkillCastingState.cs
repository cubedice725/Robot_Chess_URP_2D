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
        if (start && !Instance.action.UpdateLookAtTarget(new Vector3Int(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y, 0), transform.GetChild(0), 0.001f, 7f))
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position = Instance.PlayerPositionInt;
            start = false;
        }
        else if (!start && Instance.action.TurnAngle(new Vector3(0, 0, Instance.action.LeftAbj(transform.GetChild(0), 90)), transform.GetChild(0), 7f))
        {
            monsterMovement.IdleState();
        }
    }
    public bool Exit()
    {
        start = true;
        return true;
    }
}
