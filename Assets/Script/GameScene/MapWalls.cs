using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
public class MapWalls : MonoBehaviour
{
    private void Start()
    {
        int index = 0;
        int count = 0;
        MyObject wallObject;

        //100
        //100
        //100
        for (index = 0; index < Instance.MapSizeY; index++)
        {
            wallObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.WallObject).Get();
            wallObject.transform.position = new Vector3Int(0, index, 0);
            wallObject.transform.parent = transform;
            count++;
        }
        //101
        //101
        //101
        for (index = 0; index < Instance.MapSizeY; index++)
        {
            wallObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.WallObject).Get();
            wallObject.transform.position = new Vector3Int(Instance.MapSizeX - 1, index, 0);
            wallObject.transform.parent = transform;
            count++;
        }
        //101
        //101
        //111
        for (index = 1; index < Instance.MapSizeX - 1; index++)
        {
            wallObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.WallObject).Get();
            wallObject.transform.position = new Vector3Int(index, 0, 0);
            wallObject.transform.parent = transform;
            count++;
        }
        //111
        //101
        //111
        for (index = 1; index < Instance.MapSizeX - 1; index++)
        {
            wallObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.WallObject).Get();
            wallObject.transform.position = new Vector3Int(index, Instance.MapSizeY - 1, 0);
            wallObject.transform.parent = transform;
            count++;
        }
    }
}
