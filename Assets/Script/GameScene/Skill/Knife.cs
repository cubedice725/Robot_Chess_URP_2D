using UnityEngine;
using static GameManager;

public class Knife : CloseSkill, IState
{
    bool start = false;
    bool skillUse = false;
    public override int UsageLimit { get => 1; set { } }

    public void Entry()
    {
        AttackRange(1);
    }
    public void IStateUpdate()
    {
        MyObject crashBoxObject;
        if (UpdateSelectionCheck())
        {
            Instance.playerSkillUse = true;
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);

            crashBoxObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get();
            crashBoxObject.transform.position = new Vector3Int(Instance.hit.positionInt.x, Instance.hit.positionInt.y, 0);
            playerMovement.LookMonsterAnimation(Instance.hit.positionInt.x);
            skillUse = true;
            start = true;
        }
        if (start)
        {
            if (!UpdateLookAtTarget(Instance.hit.positionInt, 0.001f, 7f))
            {
                start = false;
            }
        }
        else if (skillUse)
        {
            if (SkillArray(new Vector3(0,0,LeftAbj(90)), 7f))
            {
                Instance.playerState = State.Idle;
                CheckUsage();
            }
        }
    }
    public bool Exit()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);

        if (skillUse)
        {
            Instance.playerSkillUse = false;
            skillUse = false;
            return true;
        }
        return false;
    }
}

 