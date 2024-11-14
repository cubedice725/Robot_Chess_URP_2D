using UnityEngine;
using static GameManager;

public class AK47 : LongSkill, IState
{
    
    Vector3Int target;
    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;

    public void Entry()
    {
        for (int i = 0; i < Instance.poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count; i++)
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position =
                new Vector2(Instance.poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][i].transform.position.x, Instance.poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][i].transform.position.y); 
        }
    }
    public void IStateUpdate()
    {
        if (UpdateSelectionCheck())
        {
            Instance.MyObjectActivate = true;
            Instance.hit.name = "";
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
            playerMovement.LookMonsterAnimation(Instance.hit.positionInt.x);
            skillUse = true;
            start = true;
        }

        if(start)
        {
            if(!UpdateLookAtTarget(Instance.hit.positionInt, accuracy, 7f))
            {
                Shoot(PoolManager.Prefabs.AK47Bullet);
                start = false;
            }
        }
        else if (skillUse)
        {
            if (SkillArray(Vector3.zero, 7f))
            {
                Instance.playerState = State.Idle;
            }
        }
    }
    public bool Exit()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        if (skillUse)
        {
            skillUse = false;
            return true;
        }
        return false;
    }
}
