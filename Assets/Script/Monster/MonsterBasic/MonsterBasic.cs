using UnityEngine;

[RequireComponent(typeof(MonsterBasicMovingState))]
[RequireComponent(typeof(MonsterBasicIdleState))]
[RequireComponent(typeof(MonsterBasicSkillCastingState))]
[RequireComponent(typeof(MonsterBasicMovement))]
public class MonsterBasic : Monster
{
    public bool start = true;    
    public override void UpdateMonster()
    {
        if (GameManager.Instance.monsterTurn)
        {
            if (start)
            {
                if (monsterMovement.AttackNavigation())
                {
                    monsterState = MonsterState.Skill;
                }
                else
                {
                    monsterState = MonsterState.Move;
                }
                start = false;
            }
        }
        else
        {
            start = true;
        }
    }
}
