using UnityEngine;
using UnityEngine.Pool;

public abstract class SkillObject : MonoBehaviour
{
    private IObjectPool<SkillObject> _ManagedPool;
    public Vector3 Direction { get; set; }
    public  void SetManagedPool(IObjectPool<SkillObject> pool)
    {
        _ManagedPool = pool;
    }
    public void Destroy()
    {
        _ManagedPool.Release(this);
    }
}
