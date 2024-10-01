using UnityEngine;
using UnityEngine.Pool;

public class MyObjectPool: MonoBehaviour
{
    public ObjectPool<MyObject> pool;
    public GameObject myObjectlPrefab;
    public void Initialize(string prefabObject, int _maxSize)
    {
        myObjectlPrefab = Resources.Load(prefabObject, typeof(GameObject)) as GameObject;
        pool = new ObjectPool<MyObject>
            (
            CreateMyObject,
            OnGetMyObject,
            OnReleaseMyObject,
            OnDestroyMyObject,
            maxSize: _maxSize
            );
    }
    protected MyObject CreateMyObject()
    {
        MyObject myObject = Instantiate(myObjectlPrefab).GetComponent<MyObject>();
        myObject.SetManagedPool(pool);
        return myObject;
    }
    protected void OnGetMyObject(MyObject myObject)
    {
        myObject.gameObject.SetActive(true);
    }
    protected void OnReleaseMyObject(MyObject myObject)
    {
        myObject.gameObject.SetActive(false);
    }
    protected void OnDestroyMyObject(MyObject myObject)
    {
        Destroy(myObject.gameObject);
    }
}
