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
    // ���� ���Ͱ� ������
    public virtual bool UpdateMove()
    {
        // ���� ��ã�� ���
        if (FinalNodeList.Count == 0)
            return false;

        if(count == 1)
        {
            print(Instance.Map2D[monsterPosition.x, monsterPosition.y]);

            print(monsterPosition.x+","+monsterPosition.y);
            // ���� ������̱⿡ �ڽ��� �ڸ��� ����� �ڽ��� �� ���� �̸� �����Ͽ� ��ġ�� �ʰ���
            Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.noting;
            Instance.Map2D[FinalNodeList[monster.MovingDistance].x, FinalNodeList[monster.MovingDistance].y] = (int)MapObject.moster;
            Authority(false);
        }

        // MovingDistance�� ���� �ൿ�� ����
        if (count <= monster.MovingDistance)
        {
            // �÷��̾���� �������� ��带 �� �������� �Ͽ�,
            // ��ǥ���� �밢������ �������� �Ծ� ���°� �ƴ�
            // ������ �����̴°�ó�� �ϱ����� ����
            if (updateMoveStart)
            {
                targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                updateMoveStart = false;
            }

            // ���� ��ġ���� ������
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, monster.MoveSpeed * Time.deltaTime);
            float distance = Vector2.Distance(transform.position, targetPosition);
            // ����� �Ÿ��� 0.01 �Ʒ��̸� �����̶� �Ǵ�
            if (distance > 0.01f)
            {
                // �ִϸ��̼�
                RunAnimation(true, targetPosition.x - transform.position.x);
                // ĳ���͸� �̵�
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, monster.MoveSpeed * Time.deltaTime);
                return true;
            }
            else
            {
                // �����ϸ� FinalNodeList�� ���� �ִ��� Ȯ��
                // count�� ������ 1�̰�, �������� ���� 1�� ���������� + 1�� ��
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
    // ���� ��Ÿ� Ȯ���� ���� �Լ�
    public bool AttackNavigation()
    {
        // ������ ��� �ڱ� ��ġ�� ����־�� Ž�� ����
        Instance.Map2D[monsterPosition.x, monsterPosition.y] = (int)MapObject.noting;
        
        // Ž��
        PathFinding(
            monsterPosition,
            new Vector3Int((int)Mathf.Round(Instance.player.transform.position.x), (int)Mathf.Round(Instance.player.transform.position.y), (int)Mathf.Round(Instance.player.transform.position.z)),
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

    // ������ ������ �÷��̾ �ٶ󺸴� �ִϸ��̼�
    public void LookPlayerAnimation()
    {
        float direction = Instance.player.transform.position.x - monsterPosition.x;

        if (direction == 0) return;
        if (xAxis == Mathf.Sign(direction)) return;

        xAxis = Mathf.Sign(direction);

        if (Mathf.Sign(direction) < 0)
        {
            // ���� ����
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // ������ ����
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    // �ɾ�� �ʿ��� �ִϸ��̼�
    public void RunAnimation(bool run, float direction = 0)
    {
        animator.SetBool("run", run);
        if (direction == 0) return;
        if (xAxis == Mathf.Sign(direction)) return;

        xAxis = Mathf.Sign(direction);

        if (Mathf.Sign(direction) < 0)
        {
            // ���� ����
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // ������ ����
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
