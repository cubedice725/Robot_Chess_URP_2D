using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Stage1 : MonoBehaviour
{
    GameObject monster;

    private void Awake()
    {
        monster = Resources.Load("Prefab/Monster/Robot", typeof(GameObject)) as GameObject;
    }
    public void Opening()
    {
        //GameObject MonsterObject = Instantiate(monster, new Vector3(1, 10, 0), Quaternion.Euler(Vector3.zero));
        //GameManager.Instance.spawnMonsters.Add(MonsterObject);

        //MonsterObject = Instantiate(monster, new Vector3(2, 10, 0), Quaternion.Euler(Vector3.zero));
        //GameManager.Instance.spawnMonsters.Add(MonsterObject);

        //MonsterObject = Instantiate(monster, new Vector3(3, 10, 0), Quaternion.Euler(Vector3.zero));
        //GameManager.Instance.spawnMonsters.Add(MonsterObject);

        //MonsterObject = Instantiate(monster, new Vector3(4, 10, 0), Quaternion.Euler(Vector3.zero));
        //GameManager.Instance.spawnMonsters.Add(MonsterObject);

        //MonsterObject = Instantiate(monster, new Vector3(5, 10, 0), Quaternion.Euler(Vector3.zero));
        //GameManager.Instance.spawnMonsters.Add(MonsterObject);

        //MonsterObject = Instantiate(monster, new Vector3(6, 10, 0), Quaternion.Euler(Vector3.zero));
        //GameManager.Instance.spawnMonsters.Add(MonsterObject);
    }
}
