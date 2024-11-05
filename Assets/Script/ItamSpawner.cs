using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.EditorTools;
using UnityEngine;
using static GameManager;

public class ItamSpawner : MonoBehaviour
{
    private void Update()
    {
        if (Instance.playerTurn && Instance.poolManager.myObjectLists[(int)PoolManager.Prefabs.RangedAttackObject].Count < 1)
        {
            SpawnItem(1);
        }
    }
    private void SpawnItem(int count)
    {
        for (int i = 0; i < count;)
        {
            Vector2Int ItemPosition = new Vector2Int(Random.Range(1, Instance.MapSizeX - 1), Random.Range(1, Instance.MapSizeY - 1));

            if (Instance.Map2D[ItemPosition.x, ItemPosition.y] != (int)MapObject.player)
            {
                if (Instance.Map2D[ItemPosition.x, ItemPosition.y] != (int)MapObject.moster)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.RangedAttackObject).Get().transform.position = new Vector3(ItemPosition.x, ItemPosition.y, 0);
                    i++;
                }
            }
        }
    }
}
