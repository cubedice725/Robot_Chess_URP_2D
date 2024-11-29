using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class RangedAttackObject : MonoBehaviour
{
    bool start = false;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && collision.transform.name == "Player")
        {
            start = true;
        }
    }
    private void Update()
    {
        if (start && Instance.playerState == GameManager.State.Idle)
        {
            Instance.skillState = GetComponent<IState>();
            Instance.playerState = GameManager.State.Skill;
            GetComponent<MyObject>().Destroy();
        }
    }
}
