using UnityEngine;
using static GameManager;

public class RangedAttackObject : MonoBehaviour
{
    bool start = false;
    // �����϶� �浹�Ǹ� Move���� �۵������� ���׹߻���
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
            Instance.playerState = State.Skill;
            start = false;
        }
    }
}
