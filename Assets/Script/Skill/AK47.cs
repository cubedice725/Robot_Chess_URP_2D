using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AK47 : MyObjectPool, IState
{
    PlayerMovement playerMovement;
    RaycastHit2D hit;
    float rotationSpeed = 7f;

    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;
    private void Awake()
    {
        Initialize("Prefab/SkillObject/AK47Bullet", 20);
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    public void Entry()
    {
        playerMovement.SetSelection();
    }
    public void IStateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.transform.name.StartsWith("Selection"))
            {
                GameManager.Instance.MyObjectActivate = true;
                playerMovement.RemoveSelection();
                playerMovement.LookMonsterAnimation(hit.collider.transform.position.x);
                skillUse = true;
                start = true;
            }
            else
            {
                playerMovement.IdleState();
            }
        }
        if (start)
        {
            // 타겟 방향 계산
            Vector3 direction = hit.transform.position - transform.position;
            direction.Normalize(); // 방향 벡터 정규화
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 좌측일경우 총을 반대로 돌림
            if (Mathf.Sign(transform.parent.localScale.x) < 0)
            {
                angle  += 180;
            }
            
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // 현재 회전 각도와 목표 각도 차이 계산
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // 각도 차이가 임계값보다 클 경우에만 회전
            if (angleDifference > accuracy)
            {
                // 부드럽게 회전
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // 좌측일경우 총알을 원래 방향으로 돌림
                if (Mathf.Sign(transform.parent.localScale.x) < 0)
                {
                    angle += 180;
                }

                MyObject bullet = pool.Get();
                bullet.transform.position = transform.GetChild(0).transform.position;
                bullet.transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
                start = false;
            }
        }
        else if (skillUse)
        {
            if (transform.rotation != Quaternion.Euler(new Vector3(0, 0, 0)))
            {
                transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(new Vector3(0, 0, 0)),
                rotationSpeed * Time.deltaTime);
            }
            else
            {
                playerMovement.IdleState();
            }
        }
    }
    public bool Exit()
    {
        playerMovement.RemoveSelection();

        if (skillUse)
        {
            skillUse = false;
            return true;
        }
        return false;
    }
}
