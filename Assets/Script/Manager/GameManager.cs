using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private int monsterFlagCount = 0;
    private int monsterAuthorityCount = 0;

    public bool playerSkillUse = false;
    public bool monsterTurn = false;
    public bool playerTurn = true;
    public bool changePlayerTurn = false;
    public bool changeMonsterTurn = false;
    public int monsterTurnCount = 0;
    public Player player { get; set; }
    public PoolManager poolManager { get; set; }
    public Hit hit;
    public bool ButtonLock { get; set; } = false;

    public int MapSizeX { get; set; } = 12;
    public int MapSizeY { get; set; } = 12;
    public int[,] Map2D { get; set; }
    public int GameScore { get; set; } = 0;
    public int GameTurnCount { get; set; } = 0;
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
        PlayerPositionInt = new Vector3Int(
            (int)Mathf.Round(player.transform.position.x),
            (int)Mathf.Round(player.transform.position.y),
            (int)Mathf.Round(transform.position.z)
        );

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
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hits.Length > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                print(hits[0].transform.name);

                hit.name = hits[0].collider.name;
                hit.positionInt = new Vector3Int(
                    (int)Mathf.Round(hits[0].collider.transform.position.x),
                    (int)Mathf.Round(hits[0].collider.transform.position.y),
                    (int)Mathf.Round(hits[0].collider.transform.position.z)
                );
            }
        }
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
            GameTurnCount++;
            GameScore += 10;
            monsterTurnCount = 0;
            FromMonsterToPlayer();
        }
    }
    // 몬스터 움직임 초기화
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
            // 행동이 끝나기 전까지 사망을 알수 없기에 행동을 확인 후 몬스터 사망처리
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

    // 몬스터 플래그를 내림
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
            ButtonLock = true;
            HandleDeadMonsters();
            ResetMonsterFlags();
            ResetMonsterMovements();
            FindNearbyMonsters();

            changePlayerTurn = false;
            monsterTurn = true;
            playerTurn = false;
        }

        if (changeMonsterTurn && !MyObjectActivate)
        {
            player.AttackCount = 0;
            player.MoveCount = 0;
            ButtonLock = false;
            changeMonsterTurn = false;
            monsterTurn = false;
            playerTurn = true;
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
        poolManager.AllDistroyMyObject(PoolManager.Prefabs.MovePlane);
        poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        hit.name = "";
        changePlayerTurn = true;
    }
    public void FromMonsterToPlayer()
    {
        changeMonsterTurn = true;
    }
}

