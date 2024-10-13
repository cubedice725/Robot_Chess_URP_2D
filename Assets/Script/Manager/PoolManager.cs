using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<ObjectPool<MyObject>> pools = new List<ObjectPool<MyObject>>();
    public List<GameObject> myObjectlPrefabs = new List<GameObject>();
    int Prefab;
    public void Awake()
    {
        //myObjectlPrefabs[0] = Resources.Load(prefabObject, typeof(GameObject)) as GameObject;
        
    }
    private void NewPoolAdd(int index, int _maxSize)
    {
        pools[index] = new ObjectPool<MyObject>
            (
            CreateMyObject,
            OnGetMyObject,
            OnReleaseMyObject,
            OnDestroyMyObject,
            maxSize: _maxSize
            );
    }

    private MyObject CreateMyObject()
    {
        MyObject myObject = Instantiate(myObjectlPrefabs[Prefab]).GetComponent<MyObject>();
        myObject.SetManagedPool(pools[0]);
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
