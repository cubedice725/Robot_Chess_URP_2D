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

    // 이동거리
    public int MoveDistance { get; set; } = 4;
    // 이동 속도
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
        // 반올림을 하지 않으면 움직이고 난 후 x좌표가 1.999일때 타입케스트는 1이 된다.
        playerPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));
    }
    // 플레이어 움직임 관련 함수
    //----------------------------------------------------------
    public bool UpdateMove()
    {
        if (FinalNodeList.Count == 0)
            return false;

        // 반올림을 하지 않으면 움직이고 난 후 x좌표가 1.999일때 타입케스트는 1이 된다.

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
        // 캐릭터가 최종적으로 움직이는 좌표의 거리, 좌표 오차는 여기서 수정
        if (distance > 0.01f)
        {
            RunAnimation(true, targetPosition.x - transform.position.x);
            // 캐릭터를 이동
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, PlayerMoveSpeed * Time.deltaTime);
            return true;
        }
        else
        {
            // 도착하면 FinalNodeList가 남아 있는지 확인
            // count의 시작은 1이고, 시작하자 마자 1이 증가함으로 + 1을 함
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
    
    // 플레이어 판 관련 함수
    //----------------------------------------------------------
    public void SetPlayerMovePlane()
    {
        // 실제 맵이랑 플레이어가 움직이는 임의 탐색 공간의 간격
        Vector3Int interval = new Vector3Int(playerPosition.x - MoveDistance, playerPosition.y - MoveDistance, 0);
        
        // 지름 계산
        int diameter = MoveDistance * 2 + 1;

        // 플레이어가 움직이는 공간을 먼저 추측하기 위해 이동거리를 기준으로 함
        for (int i = 0; i < diameter * diameter; i++)
        {
            // 플레이어의 공간 좌표
            int playerAreaX = (i / diameter);
            int playerAreaY = (i % diameter);

            // 실제 맵공간 좌표
            int mapAreaX = interval.x + playerAreaX;
            int mapAreaY = interval.y + playerAreaY;

            int distanceX = playerAreaX - MoveDistance;
            int distanceY = playerAreaY - MoveDistance;
            // 맵안 이고 플레이어가 아닌곳
            if ((mapAreaX > 0 && mapAreaY > 0 && mapAreaX < Instance.MapSizeX && mapAreaY < Instance.MapSizeY) && (playerAreaX != MoveDistance || playerAreaY != MoveDistance))
            {
                // 해당 공간에 벽이나 몬스터가 아닌곳
                if (Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.wall && Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.moster)
                {
                    // 플레이어가 움직일수 있는 거리 안 즉 원안에 있는지
                    if (Mathf.FloorToInt(Mathf.Sqrt((distanceX * distanceX) + (distanceY * distanceY))) <= MoveDistance)
                    {
                        // 경로 탐색
                        PathFinding(
                            playerPosition,
                            new Vector3Int(mapAreaX, mapAreaY, 0),
                            Vector3Int.zero,
                            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
                        );
                        // 경로 탐색이 잘 되었는지, 이동 거리가 적절한지
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

    // 스킬 판 관련 함수
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

    // 애니메이션 관련 함수
    //----------------------------------------------------------
    public void RunAnimation(bool run, float direction = 0)
    {
        animator.SetBool("run", run);
        if(direction != 0)
        {
            spriteRenderer.flipX = direction < 0;
        }
    }
    // 상태변환 관련 함수
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
