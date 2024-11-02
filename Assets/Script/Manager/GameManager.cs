using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Unity.Burst.CompilerServices;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Hit
{
    public string name;
    public Vector3Int positionInt;
    public Hit(string _name, Vector3Int _positionInt) 
    { 
        name = _name;
        positionInt = _positionInt;
    }
    
}
public class GameManager : MonoBehaviour
{
    // 몬스터는 활동(MovingState, SkillState)이 끝난 구간에 Authority = false로 끝남을 알려야함
    public enum State
    {
        Idle,
        Move,
        Skill
    }
    public enum MapObject
    {
        noting,
        wall,
        player,
        moster
    }
    private static GameManager _instance;

    public List<GameObject> spawnMonsters = new List<GameObject>();
    public List<GameObject> deadMonsters = new List<GameObject>();
    public List<float> monsterDistances = new List<float>();

    public State playerState = State.Idle;
    public IState skillState;

    private List<MyObject> movePlaneList = new List<MyObject>();
    private List<MyObject> selectionList = new List<MyObject>();
    private int monsterFlagCount = 0;
    private int monsterAuthorityCount = 0;

    public bool monsterTurn = false;
    public bool playerTurn = true;
    public bool changePlayerTurn = false;
    public bool changeMonsterTurn = false;
    public int monsterTurnCount = 0;
    public Player player { get; set; }
    public PoolManager poolManager { get; set; }
    public Hit hit;
    public int MapSizeX { get; set; } = 12;
    public int MapSizeY { get; set; } = 12;
    public int[,] Map2D { get; set; }
    public int GameScore { get; set; } = 0;
    public int GameturnCount { get; set; } = 0;
    // 필드에 오브젝트 존재여부 확인
    public bool MyObjectActivate  = false;
    public Vector3Int PlayerPositionInt { get; set; }
    public static GameManager Instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);

        // 맵에 대한 정보를 갱신
        Map2D = new int[MapSizeX, MapSizeY];

        for (int i = 0; i < MapSizeX * MapSizeY; i++)
        {
            Map2D[i / MapSizeY, i % MapSizeY] = (int)MapObject.noting;
        }

        hit = new Hit("", Vector3Int.zero);
        player = FindObjectOfType<Player>();
        poolManager = GetComponent<PoolManager>();
    }

    private void Update()
    {
        UpdatePlayerPosition();

        // 몬스터의 움직임의 규칙을 담당하는 알고리즘
        if (spawnMonsters.Count > 0 && monsterTurn && !spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority)
        {
            HandleMonsterTurn();
        }
        else if (spawnMonsters.Count == 0)
        {
            FromMonsterToPlayer();
        }

        HandleTurnChange();

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void UpdatePlayerPosition()
    {
        PlayerPositionInt = new Vector3Int(
            (int)Mathf.Round(player.transform.position.x),
            (int)Mathf.Round(player.transform.position.y),
            (int)Mathf.Round(transform.position.z)
        );
    }

    // 몬스터의 턴을 관리하는 함수
    private void HandleMonsterTurn()
    {
        // 몬스터가 N번 행동 할수 있도록 함
        if (monsterTurnCount < 2)
        {
            TransferAuthorityToNextMonster();
        }
        // 몬스터의 움직임이 끝난후 플레이어한테 넘기기전 단계
        else
        {
            ResetMonsterMovements();
            GameturnCount++;
            GameScore += 10;
            monsterTurnCount = 0;
            FromMonsterToPlayer();
        }
    }
    private void ResetMonsterMovements()
    {
        foreach (var monster in spawnMonsters)
        {
            monster.GetComponent<Monster>().MoveCount = 0;
            monster.GetComponent<Monster>().AttackCount = 0;
        }
    }

    // 몬스터의 권한을 확인하는 함수
    private void TransferAuthorityToNextMonster()
    {
        // 몬스터의 권한을 확인
        if (monsterAuthorityCount < spawnMonsters.Count - 1)
        {
            monsterAuthorityCount++;
            spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority = true;
        }
        // 몬스터의 권한이 끝나면 행동과 사망을 확인함
        else
        {
            CheckMonsterFlagsAndHandleDeaths();
        }
    }

    // 행동이 끝나기 전까지 사망을 알수 없기에 행동을 확인 후 몬스터 사망처리
    private void CheckMonsterFlagsAndHandleDeaths()
    {
        if (spawnMonsters[monsterFlagCount].GetComponent<Monster>().Flag)
        {
            if (monsterFlagCount < spawnMonsters.Count - 1)
            {
                monsterFlagCount++;
            }
            else
            {
                HandleDeadMonsters();
                FindNearbyMonsters();
                ResetMonsterFlags();
            }
        }
    }

    // 몬스터 사망처리 함수
    private void HandleDeadMonsters()
    {
        // 사망한 몬스터를 확인하고 처리, 역반복문은 Remove로 인해 count의 오차발생을 방지
        for (int count = spawnMonsters.Count - 1; count >= 0; count--)
        {
            if (spawnMonsters[count].GetComponent<Monster>().Die)
            {
                spawnMonsters[count].GetComponent<Monster>().Authority = true;
                deadMonsters.Add(spawnMonsters[count]);
                spawnMonsters.RemoveAt(count);
            }
        }
    }

    private void ResetMonsterFlags()
    {
        if (spawnMonsters.Count > 0)
        {
            for (monsterFlagCount = 0; monsterFlagCount < spawnMonsters.Count; monsterFlagCount++)
            {
                spawnMonsters[monsterFlagCount].GetComponent<Monster>().Flag = false;
            }
        }

        monsterFlagCount = 0;
        monsterAuthorityCount = 0;
        monsterTurnCount++;
    }

    private void HandleTurnChange()
    {
        if (changePlayerTurn && !MyObjectActivate)
        {
            FindNearbyMonsters();
            HandleDeadMonsters();
            changePlayerTurn = false;
            monsterTurn = true;
            playerTurn = false;
        }

        if (changeMonsterTurn && !MyObjectActivate)
        {
            changeMonsterTurn = false;
            monsterTurn = false;
            playerTurn = true;
        }
    }

    private void HandleMouseClick()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            this.hit.name = hit.collider.name;
            this.hit.positionInt = new Vector3Int(
                (int)Mathf.Round(hit.collider.transform.position.x),
                (int)Mathf.Round(hit.collider.transform.position.y),
                (int)Mathf.Round(hit.collider.transform.position.z)
            );
        }
    }

    // 플레이어와 가까운 몬스터 확인
    public void FindNearbyMonsters()
    {
        monsterDistances = new List<float>();
        for (int index = 0; index < spawnMonsters.Count; index++)
        {
            monsterDistances.Add(Vector2.Distance(player.transform.position, spawnMonsters[index].transform.position));
        }
        InsertionSort();
        spawnMonsters[0].GetComponent<Monster>().Authority = true;
    }
    public void InsertionSort()
    {
        int j = 0;
        float key = 0;
        GameObject gameObject = null;
        for (int i = 1; i < spawnMonsters.Count; i++)
        {
            key = monsterDistances[i];
            gameObject = spawnMonsters[i];
            for (j = i - 1; j >= 0; j--)
            {
                if (monsterDistances[j] > key)
                {
                    monsterDistances[j + 1] = monsterDistances[j];
                    spawnMonsters[j + 1] = spawnMonsters[j];
                }
                else
                {
                    break;
                }
            }

            monsterDistances[j + 1] = key;
            spawnMonsters[j + 1] = gameObject;
        }
    }
    public void FromPlayerToMonster()
    {
        changePlayerTurn = true;
    }
    public void FromMonsterToPlayer()
    {
        changeMonsterTurn = true;
    }

    public void SetMovePlane(Vector2 position)
    {
        movePlaneList.Add(Instance.poolManager.SelectPool(PoolManager.Prefabs.movePlane).Get());
        movePlaneList[movePlaneList.Count - 1].transform.position = new Vector3(position.x, position.y, 0);
    }
    public void RemoveMovePlane()
    {
        // RemoveAt으로 인해 0번째에 값들이 계속 존재함
        while (movePlaneList.Count > 0)
        {
            movePlaneList[0].Destroy();
            movePlaneList.RemoveAt(0);
        }
    }

    public void SetSelection(Vector2 position)
    {
        MyObject selectedObject = Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get();
        selectedObject.transform.position = position;
        selectionList.Add(selectedObject);
    }
    public void RemoveSelection()
    {
        // RemoveAt으로 인해 0번째에 값들이 계속 존재함
        while (selectionList.Count > 0)
        {
            selectionList[0].Destroy();
            selectionList.RemoveAt(0);
        }
    }
}

