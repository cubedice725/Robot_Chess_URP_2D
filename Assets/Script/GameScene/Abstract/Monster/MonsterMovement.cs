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
    // 실제 몬스터가 움직임
    public virtual bool UpdateMove()
    {
        // 길을 못찾는 경우
        if (FinalNodeList.Count == 0)
        {
            Authority(false);
            return false;
        }

        if (start)
        {
            // 몬스터 벽취급이기에 자신의 자리를 비운후 자신이 갈 곳을 미리 지정하여 겹치지 않게함
            Instance.Map2D[monsterPositionInt.x, monsterPositionInt.y] = (int)MapObject.noting;
            Instance.Map2D[FinalNodeList[monster.MovingDistance].x, FinalNodeList[monster.MovingDistance].y] = (int)MapObject.moster;
            Authority(false);
            start = false;
        }

        // MovingDistance을 통해 행동을 제약
        if (count <= monster.MovingDistance && !start)
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
            }
        }
        RunAnimation(false);
        count = 1;
        updateMoveStart = true;
        start = true;
        LookPlayerAnimation();
        return false;
    }
    // 공격 사거리 확인을 위한 함수
    public string AttackNavigation()
    {
        // 대각선인지, 사거리 안에 있는지 확인
        if (!IsDiagonal(new Vector2(monsterPositionInt.x, monsterPositionInt.y), new Vector2(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y))
            && (int)Mathf.Round(Vector2.Distance(Instance.player.transform.position, transform.position)) <= monster.AttackDistance)
        {
            return "AttackRange";
        }
        if (FinalNodeList.Count == 2 && IsDiagonal(new Vector2(monsterPositionInt.x, monsterPositionInt.y), new Vector2(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y)))
        {
            // 길을 못찾는 경우 플레이어 근처에 가까운 곳의 길을 유도함
            // 맵의 가로 세로 모두 돌아갈수 있도록함
            for (int index = 1; index < Instance.MapSizeX || index < Instance.MapSizeY; index++)
            {
                string type;
                // 플레이어 기준에서 대각선으로 점점 기준점을 낮추고 그 기준점으로 부터 플레이어와 가까운 포인트를 정사각형으로 만듦
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
            // 모든곳의 포인트를 찍어도 길을 못찾을 경우 길을 못찾는것으로 판정
            return "NotFindPath";
            // 새로은 길을 찾는걸 시도
        }
        if (FinalNodeList.Count == 0)
        {
            // 길을 못찾는 경우 플레이어 근처에 가까운 곳의 길을 유도함
            // 맵의 가로 세로 모두 돌아갈수 있도록함
            for (int index = 1; index < Instance.MapSizeX || index < Instance.MapSizeY; index++)
            {
                string type;
                // 플레이어 기준에서 대각선으로 점점 기준점을 낮추고 그 기준점으로 부터 플레이어와 가까운 포인트를 정사각형으로 만듦
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
            // 모든곳의 포인트를 찍어도 길을 못찾을 경우 길을 못찾는것으로 판정
            return "NotFindPath";
        }
        return "FindPath";
    }
    // 플레이어와 가까우면 대각선에 있는 몬스터가 공격하기 플레이어 앞이 아닌 플레이어 한테 다가간다.
    // 이를 방지하기 위해 해당 조건에 있는 경우 isTooClose를 ture로 하면 가만히 있지 않고 다른곳으로 이동하여 대각선이 아니게 되어 공격할 수 있다.
    string NewPathFinding(bool isTooClose)
    {
        float closeDistance = float.MaxValue;
        int num = 0;
        // 플레이어와 가까운 포인트가 존재하면
        if (Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint].Count != 0)
        {
            for (int index = 0; index < Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint].Count; index++)
            {
                // 가장 가까운 곳이 현 위치이면 그냥 대기
                if (!isTooClose && (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position.x) == monsterPositionInt.x &&
                    (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position.y) == monsterPositionInt.y)
                {
                    return "Warten";
                }
                // 가까운 곳에서 몬스터가 없는것을 확인
                if (Instance.Map2D
                    [(int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position.x),
                    (int)Mathf.Round(Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position.y)] != (int)MapObject.moster)
                {
                    float distance = Vector2.Distance(transform.position, Instance.poolManager.MyObjectLists[(int)Prefabs.PlayerPoint][index].transform.position);
                    // 가까운 포인트의 숫자를 저장
                    if (distance < closeDistance)
                    {
                        closeDistance = distance;
                        num = index;
                    }
                }
            }

            // 가장 가까운 포인트로 길을 찾음
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
        // 왼쪽과 오른쪽 테두리
        for (int indexY = 0; indexY < sizeY; indexY++)
        {
            AddPoint(0, indexY, startPosition);
            AddPoint(sizeX - 1, indexY, startPosition);
        }

        // 위쪽과 아래쪽 테두리 (모서리 중복 방지)
        for (int indexX = 1; indexX < sizeX - 1; indexX++)
        {
            AddPoint(indexX, 0, startPosition);
            AddPoint(indexX, sizeY - 1, startPosition);
        }
    }
    // 몬스터가 길을 찾을때 주로 사용
    public void MonsterPathFinding(Vector3Int target)
    {
        // 몬스터의 경우 자기 위치가 비어있어야 탐색 가능
        Instance.Map2D[monsterPositionInt.x, monsterPositionInt.y] = (int)MapObject.noting;

        // 탐색
        PathFinding(
            monsterPositionInt,
            target,
            Vector3Int.zero,
            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0),
            isWall
            );

        Instance.Map2D[monsterPositionInt.x, monsterPositionInt.y] = (int)MapObject.moster;
    }
    // 가만히 있을때 플레이어를 바라보는 애니메이션
    public void LookPlayerAnimation()
    {
        float direction = Instance.player.transform.position.x - monsterPositionInt.x;

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
    bool IsDiagonal(Vector2 point1, Vector2 point2)
    {
        Vector2 direction = point2 - point1;
        direction.Normalize();
        float threshold = 0.01f; // 오차 허용 범위

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
