using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static GameManager;

public class PlayerMovement : AStar
{
    private IObjectPool<PlayerMovePlane> playerMovePlanePool;
    private GameObject playerMovePlanePrefab;
    private GameObject playerPlaneStandard;
    private List<PlayerMovePlane> playerMovePlaneList = new List<PlayerMovePlane>();
    
    private IObjectPool<SkillSelection> SkillSelectionPool;
    private GameObject SkillSelectionPrefab;
    private List<SkillSelection> skillSelectionList = new List<SkillSelection>();

    Vector2 targetPosition;

    bool start = true;
    int count = 1;
    Animator animator;
    private SkillBasic skillBasic;

    RaycastHit2D hit;
    // 이동거리
    public int MovingDistance { get; set; } = 10000;
    Vector3Int playerPosition;

    protected void Awake()
    {
        playerPlaneStandard = GameObject.Find("PlayerPlaneStandard");
        playerMovePlanePrefab = Resources.Load("Prefab/Player/PlayerMovePlane", typeof(GameObject)) as GameObject;
        playerMovePlanePool = new ObjectPool<PlayerMovePlane>
            (
            CreatePlayerMovePlane,
            OnGetPlayerMovePlane,
            OnReleasePlayerMovePlane,
            OnDestroyPlayerMovePlane,
            maxSize: 500
            );

        SkillSelectionPrefab = Resources.Load("Prefab/Skill/SkillSelection", typeof(GameObject)) as GameObject;
        SkillSelectionPool = new ObjectPool<SkillSelection>
            (
            CreateSkillSelection,
            OnGetSkillSelection,
            OnReleaseSkillSelection,
            OnDestroySkillSelection,
            maxSize: 20
            );

        animator = GetComponent<Animator>();
        skillBasic = GetComponent<SkillBasic>();
    }
    public void OnClickSkillTest()
    {
        if (Instance.skillState == null)
        {
            Instance.playerState = PlayerState.Skill;
            Instance.skillState = skillBasic;
        }
    }

    // 플레이어 움직임 관련 함수
    //----------------------------------------------------------
    public bool UpdateMove()
    {
        if (FinalNodeList.Count == 0)
            return false;

        playerPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));

        if (start)
        {
            Instance.Map2D[playerPosition.x, playerPosition.y] = (int)MapObject.noting;
            PathFinding(
                new Vector3Int(playerPosition.x, playerPosition.y, 0),
                new Vector3Int((int)hit.transform.position.x, (int)hit.transform.position.y, 0),
                Vector3Int.zero,
                new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
            );
            targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
            animator.SetBool("run", true);
            start = false;
        }

        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance > 0.01f)
        {
            // 캐릭터를 이동
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, 1f * Time.deltaTime);
            return true;
        }
        else
        {
            if (count + 1< FinalNodeList.Count)
            {
                count++;
                targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                return true;
            }
            else
            {
                playerPlaneStandard.transform.position = new Vector3(playerPosition.x, playerPosition.y, 0);
                Instance.Map2D[playerPosition.x, playerPosition.y] = (int)MapObject.player;
                count = 1;
                start = true;
                animator.SetBool("run", false); 
                Instance.playerState = PlayerState.Idle;
                return false;
            }
        }
    }
    public void UpdatePlayerCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instance.Map2D[(int)transform.position.x, (int)transform.position.y] = (int)MapObject.noting;
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.name == "Player")
            {
                Instance.playerState = PlayerState.Move;
            }
        }
    }
    public bool UpdatePlayerMovePlaneCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.transform.name.StartsWith("PlayerMovePlane"))
            {
                RemovePlayerPlane();
                return true;
            }
        }
        return false;
    }
    // 플레이어 판 관련 함수
    //----------------------------------------------------------
    public void SetPlayerPlane()
    {
        playerPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));

        // 실제 맵이랑 플레이어가 움직이는 임의 탐색 공간의 편차
        Vector3Int adj = new Vector3Int(playerPosition.x - MovingDistance, playerPosition.y - MovingDistance, 0);
        
        // 지름 계산
        int diameter = MovingDistance * 2 + 1;

        for (int i = 0; i < diameter * diameter; i++)
        {
            int playerAreaX = (i / diameter);
            int playerAreaY = (i % diameter);

            int mapAreaX = adj.x + playerAreaX;
            int mapAreaY = adj.y + playerAreaY;

            int distanceX = playerAreaX - MovingDistance;
            int distanceY = playerAreaY - MovingDistance;
            // 맵안 이고 플레이어가 아닌곳
            if ((mapAreaX > 0 && mapAreaY > 0 && mapAreaX < Instance.MapSizeX && mapAreaY < Instance.MapSizeY) && (playerAreaX != MovingDistance || playerAreaY != MovingDistance))
            {
                // 해당 공간에 벽이나 몬스터가 아닌곳
                if (Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.wall && Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.moster)
                {
                    // 플레이어가 움직일수 있는 거리 안 즉 원안에 있는지
                    if (Mathf.FloorToInt(Mathf.Sqrt((distanceX * distanceX) + (distanceY * distanceY))) <= MovingDistance)
                    {
                        // 경로 탐색
                        PathFinding(
                            new Vector3Int(playerPosition.x, playerPosition.y, 0),
                            new Vector3Int(mapAreaX, mapAreaY, 0),
                            Vector3Int.zero,
                            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
                        );
                        if (FinalNodeList.Count > 1 && FinalNodeList.Count <= MovingDistance + 1)
                        {
                            playerMovePlaneList.Add(playerMovePlanePool.Get());
                            playerMovePlaneList[playerMovePlaneList.Count - 1].transform.parent = playerPlaneStandard.transform;
                            playerMovePlaneList[playerMovePlaneList.Count - 1].transform.position = new Vector3(mapAreaX, mapAreaY, 0);
                        }
                    }
                }
            }
        }
    }
    public void RemovePlayerPlane()
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
            skillSelectionList.Add(SkillSelectionPool.Get());
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

    // SkillSelection Pool 관련 함수
    //----------------------------------------------------------
    private SkillSelection CreateSkillSelection()
    {
        SkillSelection Selection = Instantiate(SkillSelectionPrefab).GetComponent<SkillSelection>();
        Selection.SetManagedPool(SkillSelectionPool);
        return Selection;
    }
    private void OnGetSkillSelection(SkillSelection Selection)
    {
        Selection.gameObject.SetActive(true);
    }
    private void OnReleaseSkillSelection(SkillSelection Selection)
    {
        Selection.gameObject.SetActive(false);
    }
    private void OnDestroySkillSelection(SkillSelection Selection)
    {
        Destroy(Selection.gameObject);
    }
    
    // PlayerMovePlane pool 관련 함수
    //----------------------------------------------------------
    private PlayerMovePlane CreatePlayerMovePlane()
    {
        PlayerMovePlane plane = Instantiate(playerMovePlanePrefab).GetComponent<PlayerMovePlane>();
        plane.SetManagedPool(playerMovePlanePool);
        return plane;
    }
    private void OnGetPlayerMovePlane(PlayerMovePlane plane)
    {
        plane.gameObject.SetActive(true);
    }
    private void OnReleasePlayerMovePlane(PlayerMovePlane plane)
    {
        plane.gameObject.SetActive(false);
    }
    private void OnDestroyPlayerMovePlane(PlayerMovePlane plane)
    {
        Destroy(plane.gameObject);
    }
}
