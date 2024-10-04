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
    // ���� ���Ͱ� ������
    public virtual bool UpdateMove()
    {
        // ���� ��ã�� ���
        if (FinalNodeList.Count == 0)
            return false;

        // MovingDistance�� ���� �ൿ�� ����
        if (count <= monster.MovingDistance)
        {
            monsterPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));

            Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.noting;
            if (updateMoveStart)
            {
                targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                updateMoveStart = false;
            }
            // ���� ��ġ���� ������
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
            float distance = Vector2.Distance(transform.position, targetPosition);
            // ���� ��ġ���� ������ �������� �Ÿ��� 0.1 �Ʒ��̸� �����̶� �Ǵ�
            if (distance > 0.01f)
            {
                RunAnimation(true, targetPosition.x - transform.position.x);
                // ĳ���͸� �̵�
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                return true;
            }
            else
            {
                // count�� ������ 1�̰�, �������� ���� 1�� ���������� + 1�� ��
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
    // ���� ��Ÿ� Ȯ���� ���� �Լ�
    public bool AttackNavigation()
    {
        monsterPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));

        // ������ ��� �ڱ� ��ġ�� ����־�� Ž�� ����
        Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.noting;
        
        // Ž��
        PathFinding(
            monsterPosition,
            new Vector3Int((int)Mathf.Round(player.transform.position.x), (int)Mathf.Round(player.transform.position.y), (int)Mathf.Round(player.transform.position.z)),
            Vector3Int.zero,
            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
            );
        Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.moster;

        // ��Ÿ� �ȿ� �ִ��� Ȯ��
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
