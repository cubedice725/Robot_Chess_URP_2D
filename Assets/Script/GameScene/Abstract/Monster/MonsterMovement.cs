using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.Properties;
using UnityEngine;
using static GameManager;
using static PoolManager;

public class MonsterMovement : AStar
{
    protected Animator animator;
    protected Monster monster;

    protected Vector3Int monsterPositionInt;
    protected Vector2 targetPosition;
    protected override bool AllowDiagonal => false;
    private float xAxis = 0;
    protected bool updateMoveStart = true;
    protected int count = 1;
    protected bool start = true;
    private List<MapObject> isWall = new List<MapObject>() { MapObject.wall, MapObject.moster };

    public void Awake()
    {
        monster = GetComponent<Monster>();
        animator = GetComponent<Animator>();
    }
    public void Start()
    {
        Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.moster;
    }
    public void Update()
    {
        monsterPositionInt = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));
    }
    // ���� ���Ͱ� ������
    public virtual bool UpdateMove()
    {
        // ���� ��ã�� ���
        if (FinalNodeList.Count == 0)
        {
            Authority(false);
            return false;
        }

        if (start)
        {
            // ���� ������̱⿡ �ڽ��� �ڸ��� ����� �ڽ��� �� ���� �̸� �����Ͽ� ��ġ�� �ʰ���
            Instance.Map2D[monsterPositionInt.x, monsterPositionInt.y] = (int)MapObject.noting;
            Instance.Map2D[FinalNodeList[monster.MovingDistance].x, FinalNodeList[monster.MovingDistance].y] = (int)MapObject.moster;
            Authority(false);
            start = false;
        }

        // MovingDistance�� ���� �ൿ�� ����
        if (count <= monster.MovingDistance && !start)
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
            }
        }
        RunAnimation(false);
        count = 1;
        updateMoveStart = true;
        start = true;
        LookPlayerAnimation();
        return false;
    }
    // ���� ��Ÿ� Ȯ���� ���� �Լ�
    public string AttackNavigation()
    {
        // �밢������, ��Ÿ� �ȿ� �ִ��� Ȯ��
        if (!IsDiagonal(new Vector2(monsterPositionInt.x, monsterPositionInt.y), new Vector2(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y))
            && (int)Mathf.Round(Vector2.Distance(Instance.player.transform.position, transform.position)) <= monster.AttackDistance)
        {
            return "AttackRange";
        }
        if (FinalNodeList.Count == 2 && IsDiagonal(new Vector2(monsterPositionInt.x, monsterPositionInt.y), new Vector2(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y)))
        {
            // ���� ��ã�� ��� �÷��̾� ��ó�� ����� ���� ���� ������
            // ���� ���� ���� ��� ���ư��� �ֵ�����
            for (int index = 1; index < Instance.MapSizeX || index < Instance.MapSizeY; index++)
            {
                string type;
                // �÷��̾� ���ؿ��� �밢������ ���� �������� ���߰� �� ���������� ���� �÷��̾�� ����� ����Ʈ�� ���簢������ ����
                CreateBorder((2 * index) + 1, (2 * index) + 1, Instance.PlayerPositionInt + ((Vector3Int.left + Vector3Int.down) * index));

                type = NewPathFinding(true);
                if (type == "Warten")
                {
                    Instance.poolManager.AllDistroyMyObject(Prefabs.PlayerPoint);
                    return "NotFindPath";
                }
                if (type == "FindPath")
                {
                    Instance.poolManager.AllDistroyMyObject(Prefabs.PlayerPoint);
                    return "FindPath";
                }
                Instance.poolManager.AllDistroyMyObject(Prefabs.PlayerPoint);
            }
            // ������ ����Ʈ�� �� ���� ��ã�� ��� ���� ��ã�°����� ����
            return "NotFindPath";
            // ������ ���� ã�°� �õ�
        }
        if (FinalNodeList.Count == 0)
        {
            // ���� ��ã�� ��� �÷��̾� ��ó�� ����� ���� ���� ������
            // ���� ���� ���� ��� ���ư��� �ֵ�����
            for (int index = 1; index < Instance.MapSizeX || index < Instance.MapSizeY; index++)
            {
                string type;
                // �÷��̾� ���ؿ��� �밢������ ���� �������� ���߰� �� ���������� ���� �÷��̾�� ����� ����Ʈ�� ���簢������ ����
                CreateBorder((2 * index) + 1, (2 * index) + 1, Instance.PlayerPositionInt + ((Vector3Int.left + Vector3Int.down) * index));

                type = NewPathFinding(false);
                if (type == "Warten")
                {
                    Instance.poolManager.AllDistroyMyObject(Prefabs.PlayerPoint);
                    return "NotFindPath";
                }
                if (type == "FindPath")
                {
                    Instance.poolManager.AllDistroyMyObject(Prefabs.PlayerPoint);
                    return "FindPath";
                }
                Instance.poolManager.AllDistroyMyObject(Prefabs.PlayerPoint);
            }
            // ������ ����Ʈ�� �� ���� ��ã�� ��� ���� ��ã�°����� ����
            return "NotFindPath";
        }
        return "FindPath";
    }
    // �÷��̾�� ������ �밢���� �ִ� ���Ͱ� �����ϱ� �÷��̾� ���� �ƴ� �÷��̾� ���� �ٰ�����.
    // �̸� �����ϱ� ���� �ش� ���ǿ� �ִ� ��� isTooClose�� ture�� �ϸ� ������ ���� �ʰ� �ٸ������� �̵��Ͽ� �밢���� �ƴϰ� �Ǿ� ������ �� �ִ�.
    string NewPathFinding(bool isTooClose)
    {
        float closeDistance = float.MaxValue;
        int num = 0;
        // �÷��̾�� ����� ����Ʈ�� �����ϸ�
        if (Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint].Count != 0)
        {
            for (int index = 0; index < Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint].Count; index++)
            {
                // ���� ����� ���� �� ��ġ�̸� �׳� ���
                if (!isTooClose && (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position.x) == monsterPositionInt.x &&
                    (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position.y) == monsterPositionInt.y)
                {
                    return "Warten";
                }
                // ����� ������ ���Ͱ� ���°��� Ȯ��
                if (Instance.Map2D
                    [(int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position.x),
                    (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position.y)] != (int)MapObject.moster)
                {
                    float distance = Vector2.Distance(transform.position, Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position);
                    // ����� ����Ʈ�� ���ڸ� ����
                    if (distance < closeDistance)
                    {
                        closeDistance = distance;
                        num = index;
                    }
                }
            }

            // ���� ����� ����Ʈ�� ���� ã��
            MonsterPathFinding(new Vector3Int(
                (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][num].transform.position.x),
                (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][num].transform.position.y),
                (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][num].transform.position.z)));

            if (FinalNodeList.Count == 0)
            {
                return "NotFindPath";
            }
            return "FindPath";
        }
        return "NotFindPath";
    }
    void AddPoint(int sizeX, int sizeY, Vector3Int startPosition)
    {
        if (startPosition.x + sizeX > 0 && startPosition.y + sizeY > 0 && startPosition.x + sizeX < Instance.MapSizeX - 1 && startPosition.y + sizeY < Instance.MapSizeY - 1)
        {
            Instance.poolManager.SelectPool(Prefabs.PlayerPoint).Get().transform.position = startPosition + new Vector3Int(sizeX, sizeY, 0);
        }
    }
    void CreateBorder(int sizeX, int sizeY, Vector3Int startPosition)
    {
        // ���ʰ� ������ �׵θ�
        for (int indexY = 0; indexY < sizeY; indexY++)
        {
            AddPoint(0, indexY, startPosition);
            AddPoint(sizeX - 1, indexY, startPosition);
        }

        // ���ʰ� �Ʒ��� �׵θ� (�𼭸� �ߺ� ����)
        for (int indexX = 1; indexX < sizeX - 1; indexX++)
        {
            AddPoint(indexX, 0, startPosition);
            AddPoint(indexX, sizeY - 1, startPosition);
        }
    }
    // ���Ͱ� ���� ã���� �ַ� ���
    public void MonsterPathFinding(Vector3Int target)
    {
        // ������ ��� �ڱ� ��ġ�� ����־�� Ž�� ����
        Instance.Map2D[monsterPositionInt.x, monsterPositionInt.y] = (int)MapObject.noting;

        // Ž��
        PathFinding(
            monsterPositionInt,
            target,
            Vector3Int.zero,
            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0),
            isWall
            );

        Instance.Map2D[monsterPositionInt.x, monsterPositionInt.y] = (int)MapObject.moster;
    }
    // ������ ������ �÷��̾ �ٶ󺸴� �ִϸ��̼�
    public void LookPlayerAnimation()
    {
        float direction = Instance.player.transform.position.x - monsterPositionInt.x;

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
    bool IsDiagonal(Vector2 point1, Vector2 point2)
    {
        Vector2 direction = point2 - point1;
        direction.Normalize();
        float threshold = 0.01f; // ���� ��� ����

        bool isHorizontal = Mathf.Abs(direction.x) > (1 - threshold) && Mathf.Abs(direction.y) < threshold;
        bool isVertical = Mathf.Abs(direction.y) > (1 - threshold) && Mathf.Abs(direction.x) < threshold;

        return !(isHorizontal || isVertical);
    }
    public void Die()
    {
        if (monster.state == State.Move)
        {
            Instance.Map2D[FinalNodeList[monster.MovingDistance].x, FinalNodeList[monster.MovingDistance].y] = (int)MapObject.noting;
        }

        animator.SetTrigger("die");
    }
    public void IdleState()
    {
        monster.state = State.Idle;
    }
    public void MoveState()
    {
        monster.state = State.Move;
    }
    public void SkillState()
    {
        monster.state = State.Skill;
    }
    public void Authority(bool value)
    {
        monster.Authority = value;
    }
}
