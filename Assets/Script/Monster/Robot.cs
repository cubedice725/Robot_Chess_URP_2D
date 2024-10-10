using UnityEngine;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(RobotSkillCastingState))]
public class Robot : Monster
{
    public override int MovingDistance { get => movingDistance; set => movingDistance = value; }
    public override float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    private float moveSpeed = 0.7f;
    private int movingDistance = 1;

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
            // 사거리 안에 있으면 스킬 아니면 움직임
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
