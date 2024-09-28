using UnityEngine;
using static GameManager;

public abstract class MonsterMovement : AStar
{
    int count = 1;
    protected Monster monster;
    public bool start = true; Player player;
    protected  void Awake()
    {
        monster = GetComponent<Monster>();
        player = FindObjectOfType<Player>();
    }
    // ���� ���Ͱ� ������
    public virtual bool UpdateMove()
    {
        // ���� ���� �÷��̾� ������ �Ÿ� Ȯ��
        if (FinalNodeList.Count != 0)
        {
            // MovingDistance�� ���� �ൿ�� ����
            if (count <= monster.MovingDistance)
            {
                Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.noting;
                
                // ���� ��ġ���� ������
                transform.Translate(Vector3.forward * 7f * Time.deltaTime);
                // ���� ��ġ���� ������ �������� �Ÿ��� 0.1 �Ʒ��̸� �����̶� �Ǵ�
                if (Vector3.Distance(transform.position, new Vector3(FinalNodeList[count].x, transform.position.y, FinalNodeList[count].y)) <= 0.1)
                {
                    Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z)] = (int)MapObject.moster;

                    //���� �� ��ġ�� ���� ��Ÿ��� �����ϸ� �������� ����
                    if (0 == FinalNodeList.Count - (monster.AttackDistance + 2 + count))
                    {
                        count = 1;
                        return false;
                    }
                    count++;
                }
            }
            else
            {
                count = 1;
                return false;
            }
        }
        else
        {
            Debug.Log("�÷��̾ ã���� ����");
        }
        return true;
    }
    // ���� ��Ÿ� Ȯ���� ���� �Լ�
    public bool AttackNavigation()
    {
        // ������ ��� �ڱ� ��ġ�� ����־�� Ž�� ����
        Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.noting;
        
        // Ž��
        PathFinding(
            new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z)),
            new Vector3Int((int)Mathf.Round(player.transform.position.x), (int)Mathf.Round(player.transform.position.y), (int)Mathf.Round(player.transform.position.z)),
            Vector3Int.zero,
            new Vector3Int(Instance.MapSizeX, 0, Instance.MapSizeY)
            );
        Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z)] = (int)MapObject.moster;

        // ��Ÿ� �ȿ� �ִ��� Ȯ��
        if (FinalNodeList.Count < monster.AttackDistance + 3 && FinalNodeList.Count != 0)
        {
            return true;
        }
        return false;
    }
    
}
