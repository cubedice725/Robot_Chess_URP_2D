using System.Collections;
using System.Collections.Generic;
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
        GameManager.Instance.spawnMonsters.Add(Instantiate(monster, new Vector3(7, 13, 0), Quaternion.Euler(Vector3.zero)));
        GameManager.Instance.spawnMonsters.Add(Instantiate(monster, new Vector3(3, 13, 0), Quaternion.Euler(Vector3.zero)));
    }
}
