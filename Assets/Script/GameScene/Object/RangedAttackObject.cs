using UnityEngine;
using static GameManager;

public class RangedAttackObject : MonoBehaviour
{
    bool start = false;
    // 움직일때 충돌되면 Move에서 작동함으로 버그발생함
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && collision.transform.name == "Player")
        {
            start = true;
        }
    }
    private void Update()
    {
        if (start)
        {
            Instance.summonedSkill = GetComponent<IState>();
            start = false;
        }
    }
}
