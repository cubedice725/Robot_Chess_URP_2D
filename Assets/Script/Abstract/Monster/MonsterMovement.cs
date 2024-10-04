using AnimationImporter.PyxelEdit;
using TMPro;
using UnityEngine;
using static GameManager;

public class MonsterMovement : AStar
{
    protected Animator animator;
    protected Monster monster;
    protected SpriteRenderer spriteRenderer;
    protected Player player;
    protected Vector3Int monsterPosition;
    protected Vector2 targetPosition;

    protected bool updateMoveStart = true;
    protected int count = 1;

    public float MoveSpeed { get; set; } = 1f;
    public void Awake()
    {
        monster = GetComponent<Monster>();
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // 실제 몬스터가 움직임
    public virtual bool UpdateMove()
    {
        // 길을 못찾는 경우
        if (FinalNodeList.Count == 0)
            return false;

        // MovingDistance을 통해 행동을 제약
        if (count <= monster.MovingDistance)
        {
            monsterPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));

            Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.noting;
            if (updateMoveStart)
            {
                targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                updateMoveStart = false;
            }
            // 다음 위치까지 움직임
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
            float distance = Vector2.Distance(transform.position, targetPosition);
            // 다음 위치까지 몬스터의 움직임이 거리가 0.1 아래이면 도착이라 판단
            if (distance > 0.01f)
            {
                RunAnimation(true, targetPosition.x - transform.position.x);
                // 캐릭터를 이동
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                return true;
            }
            else
            {
                // count의 시작은 1이고, 시작하자 마자 1이 증가함으로 + 1을 함
                if (count + 1 < FinalNodeList.Count)
                {
                    count++;
                    targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                    return true;
                }
                else
                {
                    RunAnimation(false);
                    Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.moster;
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
            return false;
        }
    }
    // 공격 사거리 확인을 위한 함수
    public bool AttackNavigation()
    {
        monsterPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));

        // 몬스터의 경우 자기 위치가 비어있어야 탐색 가능
        Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.noting;
        
        // 탐색
        PathFinding(
            monsterPosition,
            new Vector3Int((int)Mathf.Round(player.transform.position.x), (int)Mathf.Round(player.transform.position.y), (int)Mathf.Round(player.transform.position.z)),
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
    public void MonsterIdleState()
    {
        monster.state = Monster.State.Idle;
    }
    public void MonsterMoveState()
    {
        monster.state = Monster.State.Move;
    }
    public void MonsterSkillState()
    {
        monster.state = Monster.State.Skill;
    }
    public void RunAnimation(bool run, float direction = 0)
    {
        animator.SetBool("run", run);
        if (direction != 0)
        {
            spriteRenderer.flipX = direction < 0;
        }
    }
}
