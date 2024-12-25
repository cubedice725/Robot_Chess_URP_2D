using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static PoolManager;

public class SummoneMonsterStage
{
    public SummoneMonsterStage(int _Number, Prefabs _MonsterPrefabs) { Number = _Number; MonsterPrefabs = _MonsterPrefabs; }
    public int Number;
    public PoolManager.Prefabs MonsterPrefabs;
}
public class MonsterSpawner : MonoBehaviour
{
    public Vector3Int startPosition;

    private Vector2Int size;
    private List<MyObject> points = new List<MyObject>();
    private List<List<SummoneMonsterStage>> summoneMonsterStage = new List<List<SummoneMonsterStage>>()
    {
        new List<SummoneMonsterStage> { new SummoneMonsterStage(2, Prefabs.RobotPistol), new SummoneMonsterStage(3, Prefabs.RobotKnife) },
        new List<SummoneMonsterStage> { new SummoneMonsterStage(1, Prefabs.RobotSniper), new SummoneMonsterStage(2, Prefabs.RobotPistol), new SummoneMonsterStage(2, Prefabs.RobotKnife) },
        new List<SummoneMonsterStage> { new SummoneMonsterStage(1, Prefabs.RobotSniper), new SummoneMonsterStage(2, Prefabs.RobotPistol), new SummoneMonsterStage(4, Prefabs.RobotKnife) },
        new List<SummoneMonsterStage> { new SummoneMonsterStage(1, Prefabs.RobotSniper), new SummoneMonsterStage(4, Prefabs.RobotPistol), new SummoneMonsterStage(4, Prefabs.RobotKnife) },
        new List<SummoneMonsterStage> { new SummoneMonsterStage(2, Prefabs.RobotSniper), new SummoneMonsterStage(4, Prefabs.RobotPistol), new SummoneMonsterStage(5, Prefabs.RobotKnife) },
        new List<SummoneMonsterStage> { new SummoneMonsterStage(3, Prefabs.RobotSniper), new SummoneMonsterStage(5, Prefabs.RobotPistol), new SummoneMonsterStage(6, Prefabs.RobotKnife) },
        new List<SummoneMonsterStage> { new SummoneMonsterStage(5, Prefabs.RobotSniper), new SummoneMonsterStage(5, Prefabs.RobotPistol), new SummoneMonsterStage(8, Prefabs.RobotKnife) },
        new List<SummoneMonsterStage> { new SummoneMonsterStage(5, Prefabs.RobotSniper), new SummoneMonsterStage(7, Prefabs.RobotPistol), new SummoneMonsterStage(10, Prefabs.RobotKnife) },
        new List<SummoneMonsterStage> { new SummoneMonsterStage(7, Prefabs.RobotSniper), new SummoneMonsterStage(7, Prefabs.RobotPistol), new SummoneMonsterStage(12, Prefabs.RobotKnife) }
    };

    private bool start = false;
    bool[] isAvailable;
    List<int> RandomNum = new List<int>();

    private void Awake()
    {
        size.x = Instance.MapSizeX - 2;
        size.y = Instance.MapSizeY - 2;
    }
    void Start()
    {
        Instance.SummonedMonster.Clear();
        CreateBorder();
        if (Instance.poolManager == null)
        {
            Instance.poolManager = FindObjectOfType<PoolManager>();
        }
        isAvailable = new bool[points.Count];

        //UsageChecks();
        //print(CheckAvailableCount());
        //print(CheckMonstersSummonCount());
        //SelectSummoningArea(5);
        //print(RandomNum[0]);
        //print(RandomNum[1]);
        //print(RandomNum[2]);
        //print(RandomNum[3]);
        //print(RandomNum[4]);
        UsageChecks();
        SelectSummoningArea(1);

        for (int index = 0; index < 1; index++)
        {
            SummoningMonsters(index, Prefabs.RobotKnife);
        }

        UsageChecks();
        SelectSummoningArea(CheckMonstersSummonCount());
        PredictingMonsterSpawnAreas();
    }
    private void Update()
    {
        if (Instance.monsterTurn)
        {
            start = true;
        }

        if (start && Instance.playerTurn)
        {
            print(1);
            int monstersSummonCount = CheckMonstersSummonCount();
            if (Instance.GameTurnCount % 5 == 1)
            {
                // 여기 수정해야함
                for (int index1 = 0; index1 < monstersSummonCount; index1++)
                {
                    int summone = 0;
                    for (int index2 = 0; index2 < summoneMonsterStage[Instance.StageCount].Count;)
                    {
                        SummoningMonsters(index1, summoneMonsterStage[Instance.StageCount][index2].MonsterPrefabs);
                        summone++;
                        if(summone < summoneMonsterStage[Instance.StageCount][index2].Number)
                        {
                            index2++;
                        }
                    }
                }
                UsageChecks();
                SelectSummoningArea(monstersSummonCount);
                PredictingMonsterSpawnAreas();
            }
            start = false;
        }
    }
    private void PredictingMonsterSpawnAreas()
    {
        int AvailableCount = CheckAvailableCount();
        int MonstersSummonCount = CheckMonstersSummonCount();
        if (AvailableCount >= MonstersSummonCount)
        {
            for (int index = 0; index < MonstersSummonCount; index++)
            {
                Instance.poolManager.SelectPool(Prefabs.DamagedArea).Get().transform.position = points[RandomNum[index]].transform.position;
            }
        }
        else if (AvailableCount < MonstersSummonCount)
        {
            for (int index = 0; index < AvailableCount; index++)
            {
                Instance.poolManager.SelectPool(Prefabs.DamagedArea).Get().transform.position = points[RandomNum[index]].transform.position;
            }
        }
    }
    private void UsageChecks()
    {
        // 실제 몬스터가 있는지 혹은 플레이어가 있는지 확인
        for (int index = 0; index < points.Count; index++)
        {
            if (Instance.Map2D[(int)Mathf.Round(points[index].transform.position.x), (int)Mathf.Round(points[index].transform.position.y)] != (int)MapObject.player
                && Instance.Map2D[(int)Mathf.Round(points[index].transform.position.x), (int)Mathf.Round(points[index].transform.position.y)] != (int)MapObject.moster)
            {
                isAvailable[index] = true;
            }
            else
            {
                isAvailable[index] = false;
            }
        }
    }
    private int CheckAvailableCount()
    {
        int AvailableCount = 0;
        foreach (bool available in isAvailable) 
        {
            if (available)
            {
                AvailableCount++;
            }
        }
        return AvailableCount;
    }
    private int CheckMonstersSummonCount()
    {
        int MonstersSummonCount = 0;
        for (int index = 0; index < summoneMonsterStage[Instance.StageCount].Count; index++)
        {
            MonstersSummonCount += summoneMonsterStage[Instance.StageCount][index].Number;
        }
        return MonstersSummonCount;
    }
    private void SelectSummoningArea(int _MonstersSummonCount)
    {
        int Num;
        RandomNum.Clear();
        for (int index = 0; index < _MonstersSummonCount;)
        {
            Num = Random.Range(0, points.Count);
            if (isAvailable[Num])
            {
                RandomNum.Add(Num);
                isAvailable[Num] = false;
                index++;
            }
        }
    }
    private void SummoningMonsters(int index, Prefabs prefabs)
    {
        MyObject monsterObject = Instance.poolManager.SelectPool(prefabs).Get();
        monsterObject.transform.GetComponent<Monster>().Initialize();
        Instance.SummonedMonster.Add(monsterObject.gameObject);
        monsterObject.transform.position = points[RandomNum[index]].transform.position;
        Instance.Map2D[(int)points[RandomNum[index]].transform.position.x, (int)points[RandomNum[index]].transform.position.y] = (int)MapObject.moster;
        isAvailable[RandomNum[index]] = false;
    }
    void AddPoint(int x, int y)
    {
        points.Add(Instance.poolManager.SelectPool(Prefabs.Point).Get());
        points[points.Count - 1].transform.position = startPosition + new Vector3Int(x, y, 0);
        points[points.Count - 1].transform.parent = transform;
    }

    void CreateBorder()
    {
        // 왼쪽과 오른쪽 테두리
        for (int y = 0; y < size.y; y++)
        {
            AddPoint(0, y);
            AddPoint(size.x - 1, y);
        }

        // 위쪽과 아래쪽 테두리 (모서리 중복 방지)
        for (int x = 1; x < size.x - 1; x++)
        {
            AddPoint(x, 0);
            AddPoint(x, size.y - 1);
        }
    }
}
