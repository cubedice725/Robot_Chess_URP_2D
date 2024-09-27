using UnityEngine;
using UnityEngine.Pool;

public abstract class Skill : MonoBehaviour, IState
{
    protected IObjectPool<SkillObject> skillObjectPool;
    protected GameObject skillObjectlPrefab;
    protected string prefabObject;
    protected PlayerMovement playerMovement;

    protected virtual void Awake()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        skillObjectlPrefab = Resources.Load(prefabObject, typeof(GameObject)) as GameObject;
        skillObjectPool = new ObjectPool<SkillObject>
            (
            CreateSkillObject,
            OnGetSkillObject,
            OnReleaseSkillObject,
            OnDestroySkillObject,
            maxSize: 20
            );
    }
    public abstract void Enter();
    public abstract void IStateUpdate();
    public abstract void Exit();

    protected SkillObject CreateSkillObject()
    {
        SkillObject skillObject = Instantiate(skillObjectlPrefab).GetComponent<SkillObject>();
        skillObject.SetManagedPool(skillObjectPool);
        return skillObject;
    }
    protected void OnGetSkillObject(SkillObject skillObject)
    {
        skillObject.gameObject.SetActive(false);
    }
    protected void OnReleaseSkillObject(SkillObject skillObject)
    {
        skillObject.gameObject.SetActive(false);
    }
    protected void OnDestroySkillObject(SkillObject skillObject)
    {
        Destroy(skillObject.gameObject);
    }
}
