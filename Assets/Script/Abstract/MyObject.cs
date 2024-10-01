using System;
using UnityEngine;
using UnityEngine.Pool;

public class MyObject : MonoBehaviour
{
    private IObjectPool<MyObject> _ManagedPool;
    public  void SetManagedPool(IObjectPool<MyObject> pool)
    {
        _ManagedPool = pool;
    }
    public void Destroy()
    {
        _ManagedPool.Release(this);
    }
}
