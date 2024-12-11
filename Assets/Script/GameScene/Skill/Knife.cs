using UnityEngine;
using static GameManager;

public class Knife : CloseSkill, IState
{
    bool start = false;
    bool skillUse = false;
    public override int UsageLimit { get => 1; set { } }

    public void Entry()
    {
        UnifiedAttackRange(1, AttackType.Normal);
    }
    public void IStateUpdate()
    {
        MyObject crashBoxObject;
        if (UpdateSelectionCheck())
        {
            crashBoxObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get();
            crashBoxObject.transform.position = new Vector3Int(Instance.MyHit.positionInt.x, Instance.MyHit.positionInt.y, 0);
            skillUse = true;
            start = true;
        }
        if (start)
        {
            if (!Instance.action.UpdateLookAtTarget(Instance.MyHit.positionInt, transform, 0.001f, 7f))
            {
                start = false;
            }
        }
        else if (skillUse)
        {
            if (Instance.action.TurnAngle(new Vector3(0,0, Instance.action.LeftAbj(transform, 90)), transform,7f))
            {
                Instance.playerState = State.Idle;
                CheckUsage();
            }
        }
    }
    public bool Exit()
    {
        if (skillUse)
        {
            skillUse = false;
            return true;
        }
        return false;
    }
}

 