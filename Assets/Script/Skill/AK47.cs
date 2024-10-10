using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AK47 : MonoBehaviour, IState
{
    private MyObjectPool AK47Bullet;
    private void Awake()
    {
        AK47Bullet = GetComponent<MyObjectPool>();
        AK47Bullet.Initialize("Prefab/SkillObject/AK47Bullet", 20);
    }
    public void Entry()
    {
        MyObject bullet = AK47Bullet.pool.Get();
        bullet.transform.position = transform.GetChild(0).transform.position;
        bullet.transform.rotation = transform.GetChild(0).transform.rotation;
    }
    public void IStateUpdate()
    {
        
    }
    public bool Exit()
    {
        return false;
    }
}
