using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(RobotSkillCastingState))]
public class Robot : Monster
{
    public override int MovingDistance { get; set; } = 1;
    bool start = true;
    public override void Awake()
    {
        base.Awake();
        monsterIdleState = new RobotIdleState(monsterMovement);
        monsterMovingState = new RobotMovingState(monsterMovement);
        monsterSkillCastingState = GetComponent<RobotSkillCastingState>();
        monsterStateMachine.Initialize(monsterIdleState);
    }
    
    public override void UpdateMonster()
    {
        if (GameManager.Instance.monsterTurn && start)
        {
            if (Num != 0)
            {
                Num -= 1;
            }
        }
        if(Num == 0)
        {
            
        }
        else if ((GameManager.Instance.spawnMonsters[Num].GetComponent<Monster>().state != State.Move) || (GameManager.Instance.spawnMonsters[Num].GetComponent<Monster>().state != State.Skill)) return;
        
        // 몬스터 턴이되면 한번만 작동
        if (GameManager.Instance.monsterTurn && start)
        {
            if (monsterMovement.AttackNavigation())
            {
                state = State.Skill;
            }
            else
            {
                state = State.Move;
            }
            start = false;
        }

        // 몬스터 턴이 끝나면 한번만 작동
        if (!GameManager.Instance.monsterTurn && !start)
        {
            state = State.Idle;
            start = true;
        }
    }
}
