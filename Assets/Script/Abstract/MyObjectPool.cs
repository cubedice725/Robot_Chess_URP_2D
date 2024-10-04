using UnityEngine;
using UnityEngine.Pool;

// 사용시 주의사항
// 풀 2개를 동시에 사용시 아래 내용과 같이 사용하지 않으면 pool을 공유하게 됨
//
// ObjectPool = Instantiate(myObjectlPoolPrefab).GetComponent<MyObjectPool>();
// playerMovePlane.Initialize("Prefab/Object", 500);
// 
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
