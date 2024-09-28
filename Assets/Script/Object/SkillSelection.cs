using UnityEngine;
using UnityEngine.Pool;

public class SkillSelection : MonoBehaviour
{
    private IObjectPool<SkillSelection> _ManagedPool;

    public void SetManagedPool(IObjectPool<SkillSelection> pool)
    {
        _ManagedPool = pool;
    }
    public void Destroy()
    {
        _ManagedPool.Release(this);
    }
}
