using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public enum Prefabs
    {
        movePlane,
        Selection,
        AK47Bullet,
        Point,
        WallObject,
        CrashBoxObject,
        RangedAttackObject
    }
    public List<ObjectPool<MyObject>> pools = new List<ObjectPool<MyObject>>();
    public List<GameObject> myObjectlPrefabs = new List<GameObject>();
    private int index;
    public void Awake()
    {
        NewPoolAdd("Prefab/Object/movePlane", 500);
        NewPoolAdd("Prefab/Object/Selection", 20);
        NewPoolAdd("Prefab/SkillObject/AK47Bullet", 20);
        NewPoolAdd("Prefab/Object/Point", 20);
        NewPoolAdd("Prefab/Object/WallObject", 100);
        NewPoolAdd("Prefab/Object/CrashBoxObject", 100);
        NewPoolAdd("Prefab/Object/RangedAttackObject", 10);
    }
    public ObjectPool<MyObject> SelectPool(Prefabs prefabs)
    {
        index = (int)prefabs;
        return pools[index];
    }
    private void NewPoolAdd(string prefabs, int _maxSize)
    {
        myObjectlPrefabs.Add(Resources.Load(prefabs, typeof(GameObject)) as GameObject);
        pools.Add(new ObjectPool<MyObject>
            (
            CreateMyObject,
            OnGetMyObject,
            OnReleaseMyObject,
            OnDestroyMyObject,
            maxSize: _maxSize
            ));
    }
    private MyObject CreateMyObject()
    {
        MyObject myObject = Instantiate(myObjectlPrefabs[index]).GetComponent<MyObject>();
        myObject.SetManagedPool(pools[index]);
        return myObject;
    }
    private void OnGetMyObject(MyObject myObject)
    {
        myObject.gameObject.SetActive(true);
    }
    private void OnReleaseMyObject(MyObject myObject)
    {
        myObject.gameObject.SetActive(false);
    }
    private void OnDestroyMyObject(MyObject myObject)
    {
        Destroy(myObject.gameObject);
    }
}
