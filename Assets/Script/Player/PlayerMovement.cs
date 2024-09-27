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
    // �̵��Ÿ�
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

    // �÷��̾� ������ ���� �Լ�
    //----------------------------------------------------------
    public bool UpdateLooking(Vector3 target)
    {
        float stopAngle = 1.0f;
        Vector3 direction = target - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        // ��ǥ�� �ٶ󺸴� ����
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
        print("������");
    }

    // �÷��̾� �� ���� �Լ�
    //----------------------------------------------------------
    public void SetPlayerPlane()
    {
        // ���� ���̶� �÷��̾ �����̴� ���� Ž�� ������ ����
        Vector3Int adj = new Vector3Int((int)playerPlaneStandard.transform.position.x - MovingDistance, (int)playerPlaneStandard.transform.position.y - MovingDistance, 0);
        
        // ���� ���
        int diameter = MovingDistance * 2 + 1;

        for (int i = 0; i < diameter * diameter; i++)
        {
            int playerAreaX = (i / diameter);
            int playerAreaY = (i % diameter);

            int mapAreaX = adj.x + playerAreaX;
            int mapAreaY = adj.y + playerAreaY;

            int distanceX = playerAreaX - MovingDistance;
            int distanceY = playerAreaY - MovingDistance;
            // �� ���̰� �÷��̾ �ƴѰ�
            if ((mapAreaX > 0 && mapAreaY > 0 && mapAreaX < Instance.MapSizeX && mapAreaY < Instance.MapSizeY) && (playerAreaX != MovingDistance || playerAreaY != MovingDistance))
            {
                // �ش� ������ ���̳� ���Ͱ� �ƴѰ�
                if (Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.wall && Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.moster)
                {
                    // �÷��̾ �����ϼ� �ִ� �Ÿ� �� �� ���ȿ� �ִ���
                    if (Mathf.FloorToInt(Mathf.Sqrt((distanceX * distanceX) + (distanceY * distanceY))) <= MovingDistance)
                    {
                        print(mapAreaX);
                        print(mapAreaY);
                        // ��� Ž��
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

    // ��ų �� ���� �Լ�
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

    // Astar ���� �Լ�
    //----------------------------------------------------------
    protected override bool OpenListAddCondition(int checkX, int checkZ)
    {
        // �÷��̾ �����̴� �����¿� ������ ����� �ʰ�
        if (checkX >= bottomLeft.x && checkX < topRight.x && checkZ >= bottomLeft.z && checkZ < topRight.z)
        {
            // ���� ����� �ʰ�
            if ((int)transform.position.x + (checkX - MovingDistance) < Instance.MapSizeX && (int)transform.position.z + (checkZ - MovingDistance) < Instance.MapSizeY)
            {
                if ((int)transform.position.x + (checkX - MovingDistance) >= 0 && (int)transform.position.z + (checkZ - MovingDistance) >= 0)
                {
                    // ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
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
        // NodeArray�� ũ�� �����ְ�, isWall, x, z ����
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

        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.z - bottomLeft.z];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.z - bottomLeft.z];
    }
    
    // SkillSelection Pool ���� �Լ�
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
    
    // PlayerMovePlane pool ���� �Լ�
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
