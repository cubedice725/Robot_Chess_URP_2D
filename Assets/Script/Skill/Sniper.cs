using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Sniper : LongSkill, IState
{
    PlayerMovement playerMovement;

    Vector3Int target;
    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;

    private void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    public void Entry()
    {
        print("½ÇÇà");
        AttackRange(3);
    }
    public void IStateUpdate()
    {
        if (UpdateSelectionCheck())
        {
            Instance.MyObjectActivate = true;
            Instance.hit.name = "";
            Instance.RemoveSelection();
            playerMovement.LookMonsterAnimation(Instance.hit.positionInt.x);
            skillUse = true;
            start = true;
        }

        if (start)
        {
            if (!UpdateLookAtTarget(Instance.hit.positionInt, accuracy, 7f))
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
        Instance.RemoveSelection();
        if (skillUse)
        {
            skillUse = false;
            return true;
        }
        return false;
    }
}
