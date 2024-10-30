using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static GameManager;

public class MonsterSpawner : MonoBehaviour
{
    public Vector3Int startPosition;

    private Vector2Int size;
    private List<MyObject> points = new List<MyObject>();
    private GameObject monster;
    
    private void Awake()
    {
        monster = Resources.Load("Prefab/Monster/Robot", typeof(GameObject)) as GameObject;
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
            points.Add(Instance.poolManager.SelectPool(PoolManager.Prefabs.Point).Get());
            points[count].transform.position = startPosition + new Vector3Int(0, index, 0);
            points[count].transform.parent = transform;
            count++;
        }
        //101
        //101
        //101
        for (index = 0; index < size.y; index++)
        {
            points.Add(Instance.poolManager.SelectPool(PoolManager.Prefabs.Point).Get());
            points[count].transform.position = startPosition + new Vector3Int(size.x - 1, index, 0);
            points[count].transform.parent = transform;
            count++;
        }
        //101
        //101
        //111
        for (index = 1; index < size.x - 1; index++)
        {
            points.Add(Instance.poolManager.SelectPool(PoolManager.Prefabs.Point).Get());
            points[count].transform.position = startPosition + new Vector3Int(index, 0, 0);
            points[count].transform.parent = transform;
            count++;
        }
        //111
        //101
        //111
        for (index = 1; index < size.x - 1; index++)
        {
            points.Add(Instance.poolManager.SelectPool(PoolManager.Prefabs.Point).Get());
            points[count].transform.position = startPosition + new Vector3Int(index, size.y - 1, 0);
            points[count].transform.parent = transform;
            count++;
        }
    }
    private void Update()
    {
        if(Instance.spawnMonsters.Count == 0)
        {
            SpawnMonster();
            SpawnMonster();
            SpawnMonster();
            SpawnMonster();
            SpawnMonster();
            SpawnMonster();
        }
    }
    private void SpawnMonster()
    {
        GameObject MonsterObject = Instantiate(monster, points[Random.Range(0, points.Count)].transform.position, Quaternion.Euler(Vector3.zero));
        Instance.spawnMonsters.Add(MonsterObject);
    }
}
