using System.Threading;
using UnityEditor.VisionOS;
using UnityEngine;
using static GameManager;

public class MonsterMovement : AStar
{
    protected Animator animator;
    protected Monster monster;

    protected Vector3Int monsterPosition;
    protected Vector2 targetPosition;

    private float xAxis = 0;
    protected bool updateMoveStart = true;
    protected int count = 1;

    public void Awake()
    {
        monster = GetComponent<Monster>();
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        monsterPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));
    }
    // 실제 몬스터가 움직임
    public virtual bool UpdateMove()
    {
        // 길을 못찾는 경우
        if (FinalNodeList.Count == 0)
            return false;

        if(count == 1)
        {
            print(Instance.Map2D[monsterPosition.x, monsterPosition.y]);

            print(monsterPosition.x+","+monsterPosition.y);
            // 몬스터 벽취급이기에 자신의 자리를 비운후 자신이 갈 곳을 미리 지정하여 겹치지 않게함
            Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.noting;
            Instance.Map2D[FinalNodeList[monster.MovingDistance].x, FinalNodeList[monster.MovingDistance].y] = (int)MapObject.moster;
            Authority(false);
        }

        // MovingDistance을 통해 행동을 제약
        if (count <= monster.MovingDistance)
        {
            // 플레이어까지 가기위한 노드를 한 지점으로 하여,
            // 목표까지 대각선으로 막힌곳을 뚤어 가는게 아닌
            // 실제로 움직이는것처럼 하기위한 시작
            if (updateMoveStart)
            {
                targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                updateMoveStart = false;
            }

            // 다음 위치까지 움직임
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, monster.MoveSpeed * Time.deltaTime);
            float distance = Vector2.Distance(transform.position, targetPosition);
            // 노드의 거리가 0.01 아래이면 도착이라 판단
            if (distance > 0.01f)
            {
                // 애니메이션
                RunAnimation(true, targetPosition.x - transform.position.x);
                // 캐릭터를 이동
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, monster.MoveSpeed * Time.deltaTime);
                return true;
            }
            else
            {
                // 도착하면 FinalNodeList가 남아 있는지 확인
                // count의 시작은 1이고, 시작하자 마자 1이 증가함으로 + 1을 함
                if (count + 2 < FinalNodeList.Count)
                {
                    count++;
                    targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                    return true;
                }
                else
                {
                    RunAnimation(false);
                    count = 1;
                    updateMoveStart = true;
                    return false;
                }
            }
        }
        else
        {
            RunAnimation(false);
            count = 1;
            updateMoveStart = true;
            LookPlayerAnimation();
            return false;
        }
    }
    // 공격 사거리 확인을 위한 함수
    public bool AttackNavigation()
    {
        // 몬스터의 경우 자기 위치가 비어있어야 탐색 가능
        Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.noting;
        
        // 탐색
        PathFinding(
            monsterPosition,
            new Vector3Int((int)Mathf.Round(Instance.player.transform.position.x), (int)Mathf.Round(Instance.player.transform.position.y), (int)Mathf.Round(Instance.player.transform.position.z)),
            Vector3Int.zero,
            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
            );

        Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.moster;
        // 사거리 안에 있는지 확인
        if (FinalNodeList.Count < monster.AttackDistance + 3 && FinalNodeList.Count != 0)
        {
            return true;
        }
        return false;
    }

    // 가만히 있을때 플레이어를 바라보는 애니메이션
    public void LookPlayerAnimation()
    {
        float direction = Instance.player.transform.position.x - monsterPosition.x;

        if (direction == 0) return;
        if (xAxis == Mathf.Sign(direction)) return;

        xAxis = Mathf.Sign(direction);

        if (Mathf.Sign(direction) < 0)
        {
            // 왼쪽 방향
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // 오른쪽 방향
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    // 걸어갈때 필요한 애니메이션
    public void RunAnimation(bool run, float direction = 0)
    {
        animator.SetBool("run", run);
        if (direction == 0) return;
        if (xAxis == Mathf.Sign(direction)) return;

        xAxis = Mathf.Sign(direction);

        if (Mathf.Sign(direction) < 0)
        {
            // 왼쪽 방향
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // 오른쪽 방향
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    public void Die()
    {
        if (monster.state == Monster.State.Move)
        {
            Instance.Map2D[FinalNodeList[monster.MovingDistance].x, FinalNodeList[monster.MovingDistance].y] = (int)MapObject.noting;
        }

        animator.SetTrigger("die");
    }
    public void IdleState()
    {
        monster.state = Monster.State.Idle;
    }
    public void MoveState()
    {
        monster.state = Monster.State.Move;
    }
    public void SkillState()
    {
        monster.state = Monster.State.Skill;
    }
    public void Authority(bool value)
    {
        monster.Authority = value;
    }
}
