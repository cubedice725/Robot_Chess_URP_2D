using UnityEngine;
using UnityEngine.Pool;

public class PlayerMovePlane : MonoBehaviour
{
    private IObjectPool<PlayerMovePlane> _ManagedPool;

    public void SetManagedPool(IObjectPool<PlayerMovePlane> pool)
    {
        _ManagedPool = pool;
    }
    public void Destroy()
    {
        _ManagedPool.Release(this);
    }
}
