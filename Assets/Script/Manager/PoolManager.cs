using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    // Prefabs과 NewPoolAdd의 순서를 맞춰야함
    // 이유는 myObjectLists.Add(new List<MyObject>());
    public enum Prefabs
    {
        MovePlane,
        Selection,
        CrashBoxObject,
        RangedAttackObject,
        Point,
        PlayerPoint,
        WallObject,
        UnSelection,
        DamagedArea,
        Robot,
        RobotKnife,
        RobotPistol,
        RobotSniper,
        Pistol,
        Sniper,
        MoveDistanceUp,
        Schrotflinte,
        LaserGun,
        AK47Bullet,
        FighterPlaneShadow,
        FakeBullet,
        Guest
    }
    // 생성된 물체를 컨트롤하기 위한 리스트, 풀로 되돌아온 즉 비활성화된 오브젝트는 관리 안함
    public List<List<MyObject>> MyObjectLists { get; private set; } = new List<List<MyObject>>();
    public List<ObjectPool<MyObject>> pools = new List<ObjectPool<MyObject>>();
    public List<GameObject> myObjectlPrefabs = new List<GameObject>();

    private Prefabs prefabs;
    public void Reset()
    {
        foreach (Prefabs pref in Enum.GetValues(typeof(Prefabs)))
        {
            AllDistroyMyObject(pref);
            pools[(int)pref].Clear();
        }
    }
    public void Awake()
    {
        NewPoolAdd("Prefab/Object/movePlane", 500);
        NewPoolAdd("Prefab/Object/Selection", 20);
        NewPoolAdd("Prefab/Object/CrashBoxObject", 100);
        NewPoolAdd("Prefab/Object/RangedAttackObject", 10);
        NewPoolAdd("Prefab/Object/Point", 20);
        NewPoolAdd("Prefab/Object/PlayerPoint", 20);
        NewPoolAdd("Prefab/Object/WallObject", 100);
        NewPoolAdd("Prefab/Object/UnSelection", 100);
        NewPoolAdd("Prefab/Object/DamagedArea", 100);

        NewPoolAdd("Prefab/Monster/Robot", 50);
        NewPoolAdd("Prefab/Monster/RobotKnife", 50);
        NewPoolAdd("Prefab/Monster/RobotPistol", 50);
        NewPoolAdd("Prefab/Monster/RobotSniper", 50);

        NewPoolAdd("Prefab/Skill/Pistol", 2);
        NewPoolAdd("Prefab/Skill/Sniper", 2);
        NewPoolAdd("Prefab/Skill/MoveDistanceUp", 2);
        NewPoolAdd("Prefab/Skill/Schrotflinte", 2);
        NewPoolAdd("Prefab/Skill/LaserGun", 2);


        NewPoolAdd("Prefab/SkillObject/AK47Bullet", 20);
        NewPoolAdd("Prefab/SkillObject/FighterPlaneShadow", 2);
        NewPoolAdd("Prefab/SkillObject/FakeBullet", 50);

        NewPoolAdd("Prefab/UI/Guest", 100);
    }
    public ObjectPool<MyObject> SelectPool(Prefabs _prefabs)
    {
        prefabs = _prefabs;
        return pools[(int)_prefabs];
    }
    public void AllDistroyMyObject(Prefabs _prefabs)
    {
        // RemoveAt으로 인해 0번째에 값들이 계속 존재함
        while (MyObjectLists[(int)_prefabs].Count > 0)
        {
            MyObjectLists[(int)_prefabs][0].Destroy();
        }
    }
    private void NewPoolAdd(string prefabsName, int _maxSize)
    {
        myObjectlPrefabs.Add(Resources.Load(prefabsName, typeof(GameObject)) as GameObject);
        pools.Add(new ObjectPool<MyObject>
            (
            CreateMyObject,
            OnGetMyObject,
            OnReleaseMyObject,
            OnDestroyMyObject,
            maxSize: _maxSize
            ));
        MyObjectLists.Add(new List<MyObject>());
    }
    private MyObject CreateMyObject()
    {
        MyObject myObject = Instantiate(myObjectlPrefabs[(int)prefabs]).GetComponent<MyObject>();
        myObject.Prefabs = prefabs;
        myObject.SetManagedPool(pools[(int)prefabs]);
        return myObject;
    }
    private void OnGetMyObject(MyObject myObject)
    {
        MyObjectLists[(int)prefabs].Add(myObject);
        myObject.gameObject.SetActive(true);
    }
    private void OnReleaseMyObject(MyObject myObject)
    {
        // 돌아온 오브젝트가 무엇인지 판단
        for (int i = 0; i < MyObjectLists[(int)myObject.Prefabs].Count; i++)
        {
            // 돌아온 오브젝트가 무엇인지 확인되면 myObjectLists에서 제외시킴
            if (myObject.Equals(MyObjectLists[(int)myObject.Prefabs][i]))
            {
                myObject.gameObject.SetActive(false);
                MyObjectLists[(int)myObject.Prefabs].RemoveAt(i);
                return;
            }
        }
    }
    private void OnDestroyMyObject(MyObject myObject)
    {
        Destroy(myObject.gameObject);
    }
}
