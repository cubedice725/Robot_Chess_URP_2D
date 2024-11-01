using UnityEngine;
using static GameManager;

public class Knife : CloseSkill, IState
{
    PlayerMovement playerMovement;
    Vector3Int target;
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
            Instance.RemoveSelection();
            Instance.hit.name = "";
            playerMovement.LookMonsterAnimation(Instance.hit.positionInt.x);
            skillUse = true;
            start = true;
        }
        if (start)
        {
            UpdateLookAtTarget(Instance.hit.positionInt, accuracy, 7f);
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

