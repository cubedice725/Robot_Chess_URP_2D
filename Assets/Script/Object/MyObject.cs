using System;
using UnityEngine;
using UnityEngine.Pool;

// MyObject는 무조건 컴포넌트로 사용해야함
// MyObjectPool에 아래와 같이 선언하였기 때문
// MyObject myObject = Instantiate(myObjectlPrefab).GetComponent<MyObject>();
public class MyObject : MonoBehaviour
{
    private IObjectPool<MyObject> _ManagedPool;
    public PoolManager.Prefabs Prefabs { get; set; }
    public  void SetManagedPool(IObjectPool<MyObject> pool)
    {
        _ManagedPool = pool;
    }
    public void Destroy()
    {
        _ManagedPool.Release(this);
    }
}



