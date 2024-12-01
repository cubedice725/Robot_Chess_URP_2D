using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(RobotSniperSkillCastingState))]
public class RobotSniper : Monster
{
    public override int MovingDistance { get => 1; set { } }
    public override int AttackDistance { get => 3; set { } }
    public override float MoveSpeed { get => 0.7f; set { } }

    public bool start = true;

    protected override void Awake()
    {
        base.Awake();
        monsterIdleState = new RobotSniperIdleState(monsterMovement);
        monsterMovingState = new RobotSniperMovingState(monsterMovement);
        monsterSkillCastingState = GetComponent<RobotSniperSkillCastingState>();
        monsterStateMachine.Initialize(monsterIdleState);
    }
    protected override void UpdateMonster()
    {
        // ���� ���̵Ǹ� �ѹ��� �۵�, Authority�� ���� ������ ������� ������
        if (Instance.monsterTurn && start && Authority)
        {
            if (MoveCount < 1)
            {
                monsterMovement.MonsterPathFinding(Instance.PlayerPositionInt);
            }

            string returnType = monsterMovement.AttackNavigation();
            // ��Ÿ� �ȿ� ������ ��ų �ƴϸ� ������
            if (returnType == "AttackRange")
            {
                if (AttackCount == 1)
                {
                    TurnPass();
                    return;
                }

                state = State.Skill;
                AttackCount++;
            }
            else if (returnType == "NotFindPath")
            {
                TurnPass();
            }
            else if (returnType == "FindPath")
            {
                if (MoveCount == 1)
                {
                    TurnPass();
                    return;
                }
                state = State.Move;
                MoveCount++;
            }
            start = false;
        }
        if (Flag)
        {
            start = true;
            state = State.Idle;
        }
    }
}
