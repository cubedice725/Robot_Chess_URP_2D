using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class PlayerMovement : AStar
{
    private List<MyObject> playerMovePlaneList = new List<MyObject>();
    private MyObjectPool playerMovePlane;

    private List<MyObject> skillSelectionList = new List<MyObject>();
    private MyObjectPool selection;

    private GameObject myObjectlPoolPrefab;
    private GameObject playerPlaneStandard;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private RaycastHit2D hit;
    private Vector3Int playerPosition;
    private Vector2 targetPosition;

    private bool updateMoveStart = true;
    private int count = 1;

    // �̵��Ÿ�
    public int MoveDistance { get; set; } = 4;
    // �̵� �ӵ�
    public float PlayerMoveSpeed { get; set; } = 1f;
    protected void Awake()
    {
        playerPlaneStandard = GameObject.Find("PlayerPlaneStandard");

        myObjectlPoolPrefab = Resources.Load("Prefab/MyObjectPool", typeof(GameObject)) as GameObject;

        playerMovePlane = Instantiate(myObjectlPoolPrefab).GetComponent<MyObjectPool>();
        playerMovePlane.Initialize("Prefab/Player/PlayerMovePlane", 500);

        selection = Instantiate(myObjectlPoolPrefab).GetComponent<MyObjectPool>();
        selection.Initialize("Prefab/Skill/Selection", 20);

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        // �ݿø��� ���� ������ �����̰� �� �� x��ǥ�� 1.999�϶� Ÿ���ɽ�Ʈ�� 1�� �ȴ�.
        playerPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));
    }
    // �÷��̾� ������ ���� �Լ�
    //----------------------------------------------------------
    public bool UpdateMove()
    {
        if (FinalNodeList.Count == 0)
            return false;

        // �ݿø��� ���� ������ �����̰� �� �� x��ǥ�� 1.999�϶� Ÿ���ɽ�Ʈ�� 1�� �ȴ�.

        if (updateMoveStart)
        {
            PathFinding(
                playerPosition,
                new Vector3Int((int)hit.transform.position.x, (int)hit.transform.position.y, 0),
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
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, PlayerMoveSpeed * Time.deltaTime);
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
                playerPlaneStandard.transform.position = new Vector3(playerPosition.x, playerPosition.y, 0);
                Instance.Map2D[playerPosition.x, playerPosition.y] = (int)MapObject.player;
                count = 1;
                updateMoveStart = true;
                return false;
            }
        }
    }
    public bool UpdatePlayerCheck()
    {
        if (!Instance.playerTurn) return false;

        if (Input.GetMouseButtonDown(0))
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.name == "Player")
            {
                return true;
            }
        }
        return false;
    }
    public bool UpdatePlayerMovePlaneCheck()
    {
        if (!Instance.playerTurn) return false;

        if (Input.GetMouseButtonDown(0))
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.transform.name.StartsWith("PlayerMovePlane"))
            {
                RemovePlayerMovePlane();
                return true;
            }
        }
        return false;
    }
    
    // �÷��̾� �� ���� �Լ�
    //----------------------------------------------------------
    public void SetPlayerMovePlane()
    {
        // ���� ���̶� �÷��̾ �����̴� ���� Ž�� ������ ����
        Vector3Int interval = new Vector3Int(playerPosition.x - MoveDistance, playerPosition.y - MoveDistance, 0);
        
        // ���� ���
        int diameter = MoveDistance * 2 + 1;

        // �÷��̾ �����̴� ������ ���� �����ϱ� ���� �̵��Ÿ��� �������� ��
        for (int i = 0; i < diameter * diameter; i++)
        {
            // �÷��̾��� ���� ��ǥ
            int playerAreaX = (i / diameter);
            int playerAreaY = (i % diameter);

            // ���� �ʰ��� ��ǥ
            int mapAreaX = interval.x + playerAreaX;
            int mapAreaY = interval.y + playerAreaY;

            int distanceX = playerAreaX - MoveDistance;
            int distanceY = playerAreaY - MoveDistance;
            // �ʾ� �̰� �÷��̾ �ƴѰ�
            if ((mapAreaX > 0 && mapAreaY > 0 && mapAreaX < Instance.MapSizeX && mapAreaY < Instance.MapSizeY) && (playerAreaX != MoveDistance || playerAreaY != MoveDistance))
            {
                // �ش� ������ ���̳� ���Ͱ� �ƴѰ�
                if (Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.wall && Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.moster)
                {
                    // �÷��̾ �����ϼ� �ִ� �Ÿ� �� �� ���ȿ� �ִ���
                    if (Mathf.FloorToInt(Mathf.Sqrt((distanceX * distanceX) + (distanceY * distanceY))) <= MoveDistance)
                    {
                        // ��� Ž��
                        PathFinding(
                            playerPosition,
                            new Vector3Int(mapAreaX, mapAreaY, 0),
                            Vector3Int.zero,
                            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
                        );
                        // ��� Ž���� �� �Ǿ�����, �̵� �Ÿ��� ��������
                        if (FinalNodeList.Count > 1 && FinalNodeList.Count <= MoveDistance + 1)
                        {
                            playerMovePlaneList.Add(playerMovePlane.pool.Get());
                            playerMovePlaneList[playerMovePlaneList.Count - 1].transform.parent = playerPlaneStandard.transform;
                            playerMovePlaneList[playerMovePlaneList.Count - 1].transform.position = new Vector3(mapAreaX, mapAreaY, 0);
                        }
                    }
                }
            }
        }
    }
    public void RemovePlayerMovePlane()
    {
        if (playerMovePlaneList.Count > 0 && playerMovePlaneList[playerMovePlaneList.Count - 1].gameObject.activeSelf == true)
        {
            for (int i = 0; i < playerMovePlaneList.Count; i++)
            {
                if (playerMovePlaneList[i].gameObject.activeSelf == true)
                {
                    playerMovePlaneList[i].Destroy();
                }
            }
        }
    }

    // ��ų �� ���� �Լ�
    //----------------------------------------------------------
    public void SetSkillSelection()
    {
        for (int i = 0; i < Instance.spawnMonsters.Count; i++)
        {
            skillSelectionList.Add(selection.pool.Get());
            skillSelectionList[i].transform.position = Instance.spawnMonsters[i].transform.position;
            skillSelectionList[i].transform.parent = Instance.spawnMonsters[i].transform;
        }
    }
    public void RemoveSkillSelection()
    {
        if (skillSelectionList.Count > 0 && skillSelectionList[skillSelectionList.Count - 1].gameObject.activeSelf == true)
        {
            for (int i = 0; i < Instance.spawnMonsters.Count; i++)
            {
                if (skillSelectionList[i].gameObject.activeSelf == true)
                {
                    skillSelectionList[i].Destroy();
                }
            }
        }

    }

    // �ִϸ��̼� ���� �Լ�
    //----------------------------------------------------------
    public void RunAnimation(bool run, float direction = 0)
    {
        animator.SetBool("run", run);
        if(direction != 0)
        {
            spriteRenderer.flipX = direction < 0;
        }
    }
    // ���º�ȯ ���� �Լ�
    //----------------------------------------------------------
    public void IdleState()
    {
        Instance.playerState = State.Idle;
    }
    public void MoveState()
    {
        Instance.playerState = State.Move;
    }
    public void SkillState()
    {
        Instance.playerState = State.Skill;
    }
}
