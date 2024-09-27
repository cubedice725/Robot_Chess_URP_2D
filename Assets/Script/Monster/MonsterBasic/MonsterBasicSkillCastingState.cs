using UnityEngine;
using UnityEngine.Pool;

public class MonsterBasicSkillCastingState : MonsterSkillCastingState
{
    protected IObjectPool<SkillObject> skillObjectPool;
    protected GameObject skillObjectlPrefab;
    public override void Awake()
    {
        base.Awake();
        skillObjectlPrefab = Resources.Load("Prefab/Skill/SkillBasicObject", typeof(GameObject)) as GameObject;
        skillObjectPool = new ObjectPool<SkillObject>
            (
            CreateSkillObject,
            OnGetSkillObject,
            OnReleaseSkillObject,
            OnDestroySkillObject,
            maxSize: 20
            );
    }
    public override void Enter()
    {
        
    }
    public override void IStateUpdate()
    {
        if (!monsterMovement.UpdateLooking(player.transform.position))
        {
            var skillObject = skillObjectPool.Get();
            skillObject.transform.position = transform.TransformDirection(Vector3.forward) + transform.position;
            skillObject.Direction = transform.TransformDirection(Vector3.forward);
            skillObject.gameObject.SetActive(true);
            monster.monsterState = Monster.MonsterState.Idle;
            monster.flag = true;
        }
    }
    public override void Exit()
    {

    }
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