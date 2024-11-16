using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;
using static PoolManager;

public class MonsterSpawner : MonoBehaviour
{
    public Vector3Int startPosition;

    private Vector2Int size;
    private List<MyObject> points = new List<MyObject>();

    private int unspondedMonsters = 0;
    private bool start = false;
    private void Awake()
    {
        size.x = Instance.MapSizeX - 2;
        size.y = Instance.MapSizeY - 2;
    }
    void Start()
    {
        int index;
        int count = 0;

        //100
        //100
        //100
        for (index = 0; index < size.y; index++)
        {
            points.Add(Instance.poolManager.SelectPool(Prefabs.Point).Get());
            points[count].transform.position = startPosition + new Vector3Int(0, index, 0);
            points[count].transform.parent = transform;
            count++;
        }
        //101
        //101
        //101
        for (index = 0; index < size.y; index++)
        {
            points.Add(Instance.poolManager.SelectPool(Prefabs.Point).Get());
            points[count].transform.position = startPosition + new Vector3Int(size.x - 1, index, 0);
            points[count].transform.parent = transform;
            count++;
        }
        //101
        //101
        //111
        for (index = 1; index < size.x - 1; index++)
        {
            points.Add(Instance.poolManager.SelectPool(Prefabs.Point).Get());
            points[count].transform.position = startPosition + new Vector3Int(index, 0, 0);
            points[count].transform.parent = transform;
            count++;
        }
        //111
        //101
        //111
        for (index = 1; index < size.x - 1; index++)
        {
            points.Add(Instance.poolManager.SelectPool(Prefabs.Point).Get());
            points[count].transform.position = startPosition + new Vector3Int(index, size.y - 1, 0);
            points[count].transform.parent = transform;
            count++;
        }
        SpawnMonster(36);
    }
    private void Update()
    {
        print(Instance.Map2D[(int)Mathf.Round(points[20].transform.position.x), (int)Mathf.Round(points[20].transform.position.y)] == (int)MapObject.moster);
        if (Instance.monsterTurn)
        {
            start = true;
        }
        if(Instance.GameTurnCount % 1 == 0 && Instance.playerTurn && Instance.GameTurnCount != 0 && start)
        {
            SpawnMonster(3);
            SpawnMonster(0);
            start = false; 
        }
    }
    private void SpawnMonster(int numMonstersSummon)
    {
        int unavailableNumberCount = 0;
        bool[] verifiedNumber = new bool[points.Count];
        for (int index = 0; index < points.Count; index++)
        {
            if (Instance.Map2D[(int)Mathf.Round(points[index].transform.position.x), (int)Mathf.Round(points[index].transform.position.y)] != (int)MapObject.player
                && Instance.Map2D[(int)Mathf.Round(points[index].transform.position.x), (int)Mathf.Round(points[index].transform.position.y)] != (int)MapObject.moster)
            {
                verifiedNumber[index] = true;
            }
            else
            {
                unavailableNumberCount++;
            }
        }
        if (numMonstersSummon == 0)
        {
            if(unavailableNumberCount == points.Count)
            {
                print("점검");
                unspondedMonsters += unavailableNumberCount - points.Count;
                print("재소환" + unspondedMonsters);
                numMonstersSummon = points.Count - unavailableNumberCount;
            }
        }
        if (points.Count - unavailableNumberCount <= 0)
        {
            print("소환 못하여 다 들어감");
            unspondedMonsters += numMonstersSummon;
            print("소환못한 몬스터" + unspondedMonsters);

            return;
        }
        for (int i = 0; i < numMonstersSummon;)
        {
            if (unavailableNumberCount == points.Count)
            {
                print("소환한 몬스터" + i);
                print("들어가는 몬스터" + (numMonstersSummon - i));
                unspondedMonsters += numMonstersSummon - i;
                print("소환못한 몬스터" + unspondedMonsters);
                return;
            }
            int RandomNum = Random.Range(0, points.Count);
            if (verifiedNumber[RandomNum])
            {
                MyObject monsterObject = Instance.poolManager.SelectPool(Prefabs.Robot).Get();
                monsterObject.transform.GetComponent<Monster>().Initialize();
                Instance.SummonedMonster.Add(monsterObject.gameObject);
                monsterObject.transform.position = points[RandomNum].transform.position;
                verifiedNumber[RandomNum] = false;
                unavailableNumberCount++;
                i++;
            }
        }
    }
}
