using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class PlayerMovement : AStar
{
    private Animator animator;

    private Vector2 targetPosition;

    private bool updateMoveStart = true;
    private float xAxis = 0;
    private int count = 1;

    
    protected void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        
        
    }
    // �÷��̾� ������ ���� �Լ�
    //----------------------------------------------------------
    public bool UpdateMove()
    {
        if (updateMoveStart)
        {
            PathFinding(
                Instance.PlayerPositionInt,
                new Vector3Int(Instance.hit.positionInt.x, Instance.hit.positionInt.y, 0),
                Vector3Int.zero,
                new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
            );
            targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
            updateMoveStart = false;
        }

        float distance = Vector2.Distance(transform.position, targetPosition);
        // ĳ���Ͱ� ���������� �����̴� ��ǥ�� �Ÿ�, ��ǥ ������ ���⼭ ����
        if (distance > 0.01f)
        {
            RunAnimation(true, targetPosition.x - transform.position.x);
            // ĳ���͸� �̵�
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Instance.player.PlayerMoveSpeed * Time.deltaTime);
            return true;
        }
        else
        {
            // �����ϸ� FinalNodeList�� ���� �ִ��� Ȯ��
            // count�� ������ 1�̰�, �������� ���� 1�� ���������� + 1�� ��
            if (count + 1 < FinalNodeList.Count)
            {
                count++;
                targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                return true;
            }
            else
            {
                RunAnimation(false);
                Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y] = (int)MapObject.player;
                count = 1;
                updateMoveStart = true;
                return false;
            }
        }
    }
    public bool UpdateMovePlaneCheck()
    {
        if (!Instance.playerTurn) return false;

        if (Instance.hit != null && Instance.hit.name.StartsWith("MovePlane"))
        {
            Instance.RemoveMovePlane();
            return true;
        }
        return false;
    }
    
    
    public void MoveReady()
    {
        // ���� ���̶� �÷��̾ �����̴� ���� Ž�� ������ ����
        Vector3Int interval = new Vector3Int(Instance.PlayerPositionInt.x - Instance.player.MoveDistance, Instance.PlayerPositionInt.y - Instance.player.MoveDistance, 0);
        
        // ���� ���
        int diameter = Instance.player.MoveDistance * 2 + 1;

        // �÷��̾ �����̴� ������ ���� �����ϱ� ���� �̵��Ÿ��� �������� ��
        for (int i = 0; i < diameter * diameter; i++)
        {
            // �÷��̾��� ���� ��ǥ
            int playerAreaX = (i / diameter);
            int playerAreaY = (i % diameter);

            // ���� �ʰ��� ��ǥ
            int mapAreaX = interval.x + playerAreaX;
            int mapAreaY = interval.y + playerAreaY;

            int distanceX = playerAreaX - Instance.player.MoveDistance;
            int distanceY = playerAreaY - Instance.player.MoveDistance;
            // �ʾ� �̰� �÷��̾ �ƴѰ�
            if ((mapAreaX > 0 && mapAreaY > 0 && mapAreaX < Instance.MapSizeX && mapAreaY < Instance.MapSizeY) && (playerAreaX != Instance.player.MoveDistance || playerAreaY != Instance.player.MoveDistance))
            {
                // �ش� ������ ���̳� ���Ͱ� �ƴѰ�
                if (Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.wall && Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.moster)
                {
                    // �÷��̾ �����ϼ� �ִ� �Ÿ� �� �� ���ȿ� �ִ���
                    if (Mathf.FloorToInt(Mathf.Sqrt((distanceX * distanceX) + (distanceY * distanceY))) <= Instance.player.MoveDistance)
                    {
                        // ��� Ž��
                        PathFinding(
                            Instance.PlayerPositionInt,
                            new Vector3Int(mapAreaX, mapAreaY, 0),
                            Vector3Int.zero,
                            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
                        );
                        // ��� Ž���� �� �Ǿ�����, �̵� �Ÿ��� ��������
                        if (FinalNodeList.Count > 1 && FinalNodeList.Count <= Instance.player.MoveDistance + 1)
                        {
                            Instance.SetMovePlane(new Vector2(mapAreaX, mapAreaY));
                        }
                    }
                }
            }
        }
    }

    // �ִϸ��̼� ���� �Լ�
    //----------------------------------------------------------
    
    public void LookMonsterAnimation(float target)
    {
        float direction = target - transform.position.x;

        if (direction == 0) return;
        if (xAxis == Mathf.Sign(direction)) return;

        xAxis = Mathf.Sign(direction);

        if (Mathf.Sign(direction) < 0)
        {
            // ���� ����
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // ������ ����
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void RunAnimation(bool run, float direction = 0)
    {
        animator.SetBool("run", run);
        if (direction == 0) return;
        if (xAxis == Mathf.Sign(direction)) return;

        xAxis = Mathf.Sign(direction);

        if (Mathf.Sign(direction) < 0)
        {
            // ���� ����
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // ������ ����
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
