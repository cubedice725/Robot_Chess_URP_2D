using UnityEngine;
using static GameManager;

public class Knife : CloseSkill, IState
{
    MyObject crashBoxObject;

    Vector3 target;
    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;
    public override int UsageLimit { get => 1; set { } }

    public void Entry()
    {
        AttackRange(1);
    }
    public void IStateUpdate()
    {
        if (UpdateSelectionCheck())
        {
            Instance.playerSkillUse = true;
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
            crashBoxObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get();
            crashBoxObject.transform.position = Instance.hit.positionInt;
            Instance.hit.name = "";
            playerMovement.LookMonsterAnimation(Instance.hit.positionInt.x);
            skillUse = true;
            start = true;
        }
        if (start)
        {
            if(!UpdateLookAtTarget(Instance.hit.positionInt, accuracy, 7f))
            {
                crashBoxObject.GetComponent<CrashBoxObject>().collObject.gameObject.GetComponent<Monster>().HP -= 1;
                start = false;
                Usage++;
            }
        }
        else if (skillUse)
        {
            if (SkillArray(new Vector3(0,0,LeftAbj(90)), 7f))
            {
                crashBoxObject.Destroy();
                Instance.playerState = State.Idle;
                CheckUsage();
            }
        }
    }
    public bool Exit()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        if (skillUse)
        {
            Instance.playerSkillUse = false;
            skillUse = false;
            return true;
        }
        return false;
    }
}

 