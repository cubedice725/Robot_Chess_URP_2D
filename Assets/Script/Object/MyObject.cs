using System;
using UnityEngine;
using UnityEngine.Pool;

// MyObject�� ������ ������Ʈ�� ����ؾ���
// MyObjectPool�� �Ʒ��� ���� �����Ͽ��� ����
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



