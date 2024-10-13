using UnityEngine;

public class AK47 : MonoBehaviour, IState
{
    PlayerMovement playerMovement;
    RaycastHit2D hit;
    float rotationSpeed = 7f;

    bool start = false;
    bool skillUse = false;
    float accuracy = 0.001f;
    private void Awake()
    {
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
            // Ÿ�� ���� ���
            Vector3 direction = hit.transform.position - transform.position;
            direction.Normalize(); // ���� ���� ����ȭ
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // �����ϰ�� ���� �ݴ�� ����
            if (Mathf.Sign(transform.parent.localScale.x) < 0)
            {
                angle  += 180;
            }
            
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // ���� ȸ�� ������ ��ǥ ���� ���� ���
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // ���� ���̰� �Ӱ谪���� Ŭ ��쿡�� ȸ��
            if (angleDifference > accuracy)
            {
                // �ε巴�� ȸ��
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // �����ϰ�� �Ѿ��� ���� �������� ����
                if (Mathf.Sign(transform.parent.localScale.x) < 0)
                {
                    angle += 180;
                }

                MyObject bullet = GameManager.Instance.poolManager.SelectPool(PoolManager.Prefabs.AK47Bullet).Get();
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
