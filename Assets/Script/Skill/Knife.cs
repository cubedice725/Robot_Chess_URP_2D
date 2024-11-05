using UnityEngine;
using static GameManager;

public class Knife : CloseSkill, IState
{
    PlayerMovement playerMovement;
    MyObject myObject;
    Vector3 target;
    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;
    public void Entry()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        AttackRange(1);
    }
    public void IStateUpdate()
    {
        if (UpdateSelectionCheck())
        {
            Instance.playerSkillUse = true;
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
            myObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get();
            myObject.transform.position = Instance.hit.positionInt;
            Instance.hit.name = "";
            playerMovement.LookMonsterAnimation(Instance.hit.positionInt.x);
            skillUse = true;
            start = true;
        }
        if (start)
        {
            if(!UpdateLookAtTarget(Instance.hit.positionInt, accuracy, 7f))
            {
                myObject.GetComponent<CrashBoxObject>().collObject.gameObject.GetComponent<Monster>().HP -= 1;
                start = false;
            }
        }
        else if (skillUse)
        {
            if (SkillArray(new Vector3(0,0,LeftAbj(90)), 7f))
            {
                myObject.Destroy();
                Instance.playerState = State.Idle;
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

 