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
        GameObject MonsterObject = Instantiate(monster, new Vector3(7, 13, 0), Quaternion.Euler(Vector3.zero));
        MonsterObject.GetComponent<Monster>().Num = 0;
        GameManager.Instance.spawnMonsters.Add(MonsterObject);

        MonsterObject = Instantiate(monster, new Vector3(3, 13, 0), Quaternion.Euler(Vector3.zero));
        MonsterObject.GetComponent<Monster>().Num = 1;
        GameManager.Instance.spawnMonsters.Add(MonsterObject);
    }
}
