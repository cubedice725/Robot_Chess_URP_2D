using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
    private SkillBasic skillBasic;
    public RaycastHit Hit { get; set; }
    // 이동거리
    public int MovingDistance { get; set; } = 4;


    protected override void Awake()
    {
        base.Awake();
        playerPlaneStandard = GameObject.Find("PlayerPlaneStandard");
        playerMovePlanePrefab = Resources.Load("Prefab/Player/MovePlane", typeof(GameObject)) as GameObject;
        playerMovePlanePool = new ObjectPool<PlayerMovePlane>
            (
            CreatePlayerMovePlane,
            OnGetPlayerMovePlane,
            OnReleasePlayerMovePlane,
            OnDestroyPlayerMovePlane,
            maxSize: 500
            );

        SkillSelectionPrefab = Resources.Load("Prefab/Skill/Selection", typeof(GameObject)) as GameObject;
        SkillSelectionPool = new ObjectPool<SkillSelection>
            (
            CreateSkillSelection,
            OnGetSkillSelection,
            OnReleaseSkillSelection,
            OnDestroySkillSelection,
            maxSize: 20
            );

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
    public bool UpdateLooking(Vector3 target)
    {
        float stopAngle = 1.0f;
        Vector3 direction = target - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        // 목표를 바라보는 조건
        if (angle > stopAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.deltaTime);
        }
        else
        {
            return false;
        }
        return true;
    }
    public void Move()
    {
        Instance.Map2D[(int)transform.position.x, (int)transform.position.z] = (int)MapObject.noting;

        transform.position = new Vector3(Hit.transform.position.x, transform.position.y, Hit.transform.position.z);
        playerPlaneStandard.transform.position = new Vector3(transform.position.x, 1, transform.position.z);


        Instance.Map2D[(int)transform.position.x, (int)transform.position.z] = (int)MapObject.player;
    }
    public void UpdatePlayerCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider.gameObject.name == "Player")
            {
                Instance.playerState = PlayerState.Move;
            }
        }
    }
    
    public void Stateprint()
    {
        print("동작중");
    }

    // 플레이어 판 관련 함수
    //----------------------------------------------------------
    public void SetPlayerPlane()
    {
        // 실제 맵이랑 플레이어가 움직이는 임의 탐색 공간의 편차
        Vector3Int adj = new Vector3Int((int)playerPlaneStandard.transform.position.x - MovingDistance, (int)playerPlaneStandard.transform.position.y - MovingDistance, 0);
        
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
            // 맵 안이고 플레이어가 아닌곳
            if ((mapAreaX > 0 && mapAreaY > 0 && mapAreaX < Instance.MapSizeX && mapAreaY < Instance.MapSizeY) && (playerAreaX != MovingDistance || playerAreaY != MovingDistance))
            {
                // 해당 공간에 벽이나 몬스터가 아닌곳
                if (Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.wall && Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.moster)
                {
                    // 플레이어가 움직일수 있는 거리 안 즉 원안에 있는지
                    if (Mathf.FloorToInt(Mathf.Sqrt((distanceX * distanceX) + (distanceY * distanceY))) <= MovingDistance)
                    {
                        print(mapAreaX);
                        print(mapAreaY);
                        // 경로 탐색
                        PathFinding(
                            new Vector3Int(MovingDistance, MovingDistance, 0),
                            new Vector3Int(playerAreaX, playerAreaY, 0),
                            Vector3Int.zero,
                            new Vector3Int(MovingDistance * 2 + 1, MovingDistance * 2 + 1, 0)
                        );
                        if (FinalNodeList.Count > 1 && FinalNodeList.Count <= MovingDistance + 1)
                        {
                            playerMovePlaneList.Add(playerMovePlanePool.Get());
                            playerMovePlaneList[playerMovePlaneList.Count - 1].transform.parent = playerPlaneStandard.transform;
                            playerMovePlaneList[playerMovePlaneList.Count - 1].transform.localPosition = new Vector3(distanceX, distanceY, 0);
                        }
                    }
                }
            }
        }
    }
    public void RemovePlayerPlane()
    {
        if (playerMovePlaneList.Count > 0)
        {
            if (playerMovePlaneList[playerMovePlaneList.Count - 1].gameObject.activeSelf == true)
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
        if (skillSelectionList.Count > 0)
        {
            if (skillSelectionList[skillSelectionList.Count - 1].gameObject.activeSelf == true)
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

    }

    // Astar 관련 함수
    //----------------------------------------------------------
    protected override bool OpenListAddCondition(int checkX, int checkZ)
    {
        // 플레이어가 움직이는 상하좌우 범위를 벗어나지 않고
        if (checkX >= bottomLeft.x && checkX < topRight.x && checkZ >= bottomLeft.z && checkZ < topRight.z)
        {
            // 맵을 벗어나지 않고
            if ((int)transform.position.x + (checkX - MovingDistance) < Instance.MapSizeX && (int)transform.position.z + (checkZ - MovingDistance) < Instance.MapSizeY)
            {
                if ((int)transform.position.x + (checkX - MovingDistance) >= 0 && (int)transform.position.z + (checkZ - MovingDistance) >= 0)
                {
                    // 벽이 아니면서, 닫힌리스트에 없다면
                    if (!NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    protected override void SetPathFinding()
    {
        // NodeArray의 크기 정해주고, isWall, x, z 대입
        sizeX = topRight.x - bottomLeft.x;
        sizeY = topRight.z - bottomLeft.z;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX * sizeY; i++)
        {
            int mapObject = 0;
            int map2dX = (int)transform.position.x + ((i / (MovingDistance * 2 + 1)) - MovingDistance);
            int map2dZ = (int)transform.position.z + ((i % (MovingDistance * 2 + 1)) - MovingDistance);

            try
            {
                mapObject = Instance.Map2D[map2dX, map2dZ];
            }
            catch { }

            bool isWall = false;
            if ((int)MapObject.wall == mapObject || (int)MapObject.moster == mapObject)
            {
                isWall = true;
            }

            NodeArray[i / sizeY, i % sizeY] = new Node(isWall, (i / sizeY) + bottomLeft.x, (i % sizeY) + bottomLeft.z);
        }

        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.z - bottomLeft.z];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.z - bottomLeft.z];
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
