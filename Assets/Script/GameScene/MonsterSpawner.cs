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
        new List<SummoneMonsterStage> { new SummoneMonsterStage(3, Prefabs.RobotKnife) },
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
    bool[] available;
    List<int> randomNum = new List<int>();

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
        available = new bool[points.Count];
        AvailableChecks();
        NonDuplicatedRandom(MonstersSummonCount(Instance.StageCount));

        for (int index = 0; index < 3; index++)
        {
            SummoningMonsters(index, Prefabs.RobotKnife);
        }
        AvailableChecks();
        NonDuplicatedRandom(MonstersSummonCount(Instance.StageCount + 1));
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
            if (Instance.GameTurnCount % 5 == 1)
            {
                int summone = 0;
                int monsterindex = 0;
                int availableCount = AvailableCount();
                int monstersSummonCount = MonstersSummonCount(Instance.StageCount);
                int count = randomNum.Count;
                if (availableCount < monstersSummonCount)
                {
                    count = points.Count;
                }

                for (int index = 0; index < count; index++)
                {
                    if (summone >= summoneMonsterStage[Instance.StageCount][monsterindex].Number)
                    {
                        summone = 0;
                        monsterindex++;
                    }
                    if (availableCount < monstersSummonCount)
                    {
                        if (available[index])
                        {
                            SummoningMonsters(index, summoneMonsterStage[Instance.StageCount][monsterindex].MonsterPrefabs);
                        }
                    }
                    else
                    {
                        SummoningMonsters(randomNum[index], summoneMonsterStage[Instance.StageCount][monsterindex].MonsterPrefabs);
                    }
                    summone++;
                }
                AvailableChecks();
                NonDuplicatedRandom(MonstersSummonCount(Instance.StageCount + 1));
                PredictingMonsterSpawnAreas();
            }
            AvailableChecks();
            PredictingMonsterSpawnAreas();
            start = false;
        }
    }
    private void FixedUpdate()
    {
        AvailableChecks();

        int availableCount = AvailableCount();
        int monstersSummonCount = MonstersSummonCount(Instance.StageCount + 1);
        if (availableCount >= monstersSummonCount)
        {
            for (int index = 0; index < randomNum.Count; index++)
            {
                if (Instance.Map2D[(int)Mathf.Round(points[randomNum[index]].transform.position.x), (int)Mathf.Round(points[randomNum[index]].transform.position.y)] == (int)MapObject.player
                        || Instance.Map2D[(int)Mathf.Round(points[randomNum[index]].transform.position.x), (int)Mathf.Round(points[randomNum[index]].transform.position.y)] == (int)MapObject.moster)
                {
                    IntegrityCheck(index);
                    PredictingMonsterSpawnAreas();
                }
            }
        }
        else
        {
            PredictingMonsterSpawnAreas();
        }
    }
    private void IntegrityCheck(int changeIndex)
    {
        List<int> temp = new List<int>();
        foreach(int num in randomNum)
        {
            available[num] = false;
        }

        for (int index = 0;index < available.Length; index++)
        {
            if (available[index])
            {
                temp.Add(index);
            }
        }
        randomNum[changeIndex] = temp[Random.Range(0, temp.Count)];
    }
    private void PredictingMonsterSpawnAreas()
    {
        Instance.poolManager.AllDistroyMyObject(Prefabs.MonsterSummoningArea);
        int availableCount = AvailableCount();
        int monstersSummonCount = MonstersSummonCount(Instance.StageCount + 1);
        int count = randomNum.Count;

        if (randomNum.Count == 0)
        {
            NonDuplicatedRandom(monstersSummonCount);
        }
        if (availableCount < monstersSummonCount)
        {
            count = points.Count;
        }

        for (int index = 0; index < count; index++)
        {
            if (availableCount < monstersSummonCount)
            {
                if (available[index])
                {
                    Instance.poolManager.SelectPool(Prefabs.MonsterSummoningArea).Get().transform.position = points[index].transform.position;
                }
            }
            else
            {
                Instance.poolManager.SelectPool(Prefabs.MonsterSummoningArea).Get().transform.position = points[randomNum[index]].transform.position;

            }
        }
    }
    private void AvailableChecks()
    {
        // 실제 몬스터가 있는지 혹은 플레이어가 있는지 확인
        for (int index = 0; index < points.Count; index++)
        {
            if (Instance.Map2D[(int)Mathf.Round(points[index].transform.position.x), (int)Mathf.Round(points[index].transform.position.y)] != (int)MapObject.player
                && Instance.Map2D[(int)Mathf.Round(points[index].transform.position.x), (int)Mathf.Round(points[index].transform.position.y)] != (int)MapObject.moster)
            {
                available[index] = true;
            }
            else
            {
                available[index] = false;
            }
        }
    }
    private int AvailableCount()
    {
        int AvailableCount = 0;
        foreach (bool available in available)
        {
            if (available)
            {
                AvailableCount++;
            }
        }
        return AvailableCount;
    }
    private int MonstersSummonCount(int stage)
    {
        int MonstersSummonCount = 0;
        for (int index = 0; index < summoneMonsterStage[stage].Count; index++)
        {
            MonstersSummonCount += summoneMonsterStage[stage][index].Number;
        }
        return MonstersSummonCount;
    }
    private void NonDuplicatedRandom(int count)
    {
        randomNum.Clear();
        int availableCount = AvailableCount();
        int monstersSummonCount = MonstersSummonCount(Instance.StageCount + 1);
        if (availableCount > monstersSummonCount)
        {
            // 사용할수 있는 인덱스 값을 저장할 리스트
            List<int> temp = new List<int>();
            int index = 0;
            for (index = 0; index < available.Length; index++)
            {
                // 해당 공간을 사용할 수 있으면
                if (available[index])
                {
                    // 사용할수 있는 인덱스 값에 저장
                    temp.Add(index);
                }
            }

            for (index = 0; index < count; index++)
            {
                int num = Random.Range(0, temp.Count);
                // 뽑은 랜덤 값을 통해 available 인덱스 값을 랜덤 번호로 사용하고 
                // temp를 리스트에서 제거함으로 중복을 제거함
                randomNum.Add(temp[num]);
                temp.RemoveAt(num);
            }
        }
        else
        {
            print("랜덤 값을 가지기에는 소환할 공간이 부족하여 의미가 없다.");
        }
    }
    private void SummoningMonsters(int index, Prefabs prefabs)
    {
        MyObject monsterObject = Instance.poolManager.SelectPool(prefabs).Get();
        monsterObject.transform.GetComponent<Monster>().Initialize();
        Instance.SummonedMonster.Add(monsterObject.gameObject);
        monsterObject.transform.position = points[index].transform.position;
        Instance.Map2D[(int)points[index].transform.position.x, (int)points[index].transform.position.y] = (int)MapObject.moster;
        available[index] = false;
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
