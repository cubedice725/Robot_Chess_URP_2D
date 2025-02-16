using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    public List<float> monsterDistances = new List<float>();
    public List<GameObject> SummonedMonster = new List<GameObject>();

    public State playerState = State.Idle;
    public IState skillState;
    public IState summonedSkill;

    public bool monsterTurn = false;
    public bool playerTurn = true;
    public bool changePlayerTurn = false;
    public bool changeMonsterTurn = false;

    public int SummonedMonsterAuthorityCount = 0;
    public int monsterTurnCount = 0;
    public float time = 0;

    [SerializeField]
    public Hit MyHit { get; set; }

    public Player player { get; set; }
    public PoolManager poolManager { get; set; }
    public Action action { get; set; }
    public Vector3Int PlayerPositionInt { get; set; }
    public Vector3Int MouseDownPosInt { get; set; }
    public Vector3Int MousePosInt { get; set; }
    public Vector3 MousePos { get; set; }

    public int StageCount { get; set; } = 0;
    public int MapSizeX { get; set; } = 12;
    public int MapSizeY { get; set; } = 12;
    public int[,] Map2D { get; set; }
    public int GameScore { get; set; } = 0;
    public int GameTurnCount = 1;
    public bool ButtonLock  = false;
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
    public void Reset()
    {
        playerState = State.Idle;
        ButtonLock = false;
        monsterTurn = false;
        playerTurn = true;
        changePlayerTurn = false;
        changeMonsterTurn = false;
        monsterTurnCount = 0;
        GameScore = 0;
        GameTurnCount = 1;
        StageCount = 0;
        for (int i = 0; i < MapSizeX * MapSizeY; i++)
        {
            Map2D[i / MapSizeY, i % MapSizeY] = (int)MapObject.noting;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
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
        MyHit = new Hit("", Vector3Int.zero);
        player = FindObjectOfType<Player>();
        poolManager = FindObjectOfType<PoolManager>();
        action = GetComponent<Action>();
    }
   
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "GameScene") return;

        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        
        PlayerPositionInt = new Vector3Int(
            (int)Mathf.Round(player.transform.position.x),
            (int)Mathf.Round(player.transform.position.y),
            (int)Mathf.Round(transform.position.z)
        );
        
        // 몬스터의 움직임의 규칙을 담당하는 알고리즘
        if (monsterTurn && SummonedMonster.Count > 0)
        {
            ButtonLock = true;
            // 몬스터가 N번 행동 할수 있도록 함
            if (monsterTurnCount < 2)
            {
                if (CheckMonsterAuthority())
                {
                    if (CheckMonsterFlag())
                    {
                        MonsterDeathProcessing();
                        ResetMonsterFlags();
                        FindNearbyMonsters();
                        SummonedMonsterAuthorityCount = 0;
                        monsterTurnCount++;
                    }
                }
            }
            else
            {
                ButtonLock = false;
                GameTurnCount++;
                GameScore += 10;
                monsterTurnCount = 0;
                SummonedMonsterAuthorityCount = 0;
                ResetMonsterMovements();
                changeMonsterTurn = true;
            }
        }
        else if (SummonedMonster.Count == 0)
        {
            GameTurnCount++;
            GameScore += 10;
            monsterTurnCount = 0;
            SummonedMonsterAuthorityCount = 0;
            ResetMonsterMovements();
            changeMonsterTurn = true;
        }// 몬스터가 없음 턴이 자동적으로 올라감
        if (changePlayerTurn)
        {
            time += Time.deltaTime;
            ButtonLock = true;

            // 플레이어에서 몬트터로 넘어갈시 몬스터 사망처리 전에 동작하는것을 방지하기 위한 타임 딜레이
            if (time > 0.3f)
            {
                time = 0;
                ResetMonsterMovements();
                MonsterDeathProcessing();
                ResetMonsterFlags();
                FindNearbyMonsters();
                changePlayerTurn = false;
                monsterTurn = true;
                playerTurn = false;
            }
        }
        if (changeMonsterTurn)
        {
            player.AttackCount = 0;
            player.MoveCount = 0;
            ButtonLock = false;
            changeMonsterTurn = false;
            monsterTurn = false;
            playerTurn = true;
            if(GameTurnCount % 5 == 1)
            {
                StageCount++;
            }
        }
        
        if (player.Die) return;

        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MousePosInt = new Vector3Int(
            (int)Mathf.Round(MousePos.x),
            (int)Mathf.Round(MousePos.y),
            -1
        );
        if (Input.GetMouseButtonDown(0))
        {
            MouseDownPosInt = MousePosInt;

            RaycastHit2D[] hits = Physics2D.RaycastAll(MousePos, Vector2.zero);
            if (hits.Length > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                print(hits[0].transform.name);
            }
            try{
                MyHit.name = hits[0].collider.name;
                MyHit.positionInt = new Vector3Int(
                    (int)Mathf.Round(hits[0].collider.transform.position.x),
                    (int)Mathf.Round(hits[0].collider.transform.position.y),
                    (int)Mathf.Round(hits[0].collider.transform.position.z)
                );
            }
            catch
            {
                MyHit.name = "";
            }
        }
    }
    // 몬스터 권한 확인
    private bool CheckMonsterAuthority()
    {
        if (SummonedMonster.Count > 0)
        {
            if (!SummonedMonster[SummonedMonsterAuthorityCount].GetComponent<Monster>().Authority && SummonedMonsterAuthorityCount + 1 < SummonedMonster.Count)
            {
                SummonedMonster[SummonedMonsterAuthorityCount + 1].GetComponent<Monster>().Authority = true;
                SummonedMonsterAuthorityCount++;
            }
            if (SummonedMonsterAuthorityCount + 1 == SummonedMonster.Count)
            {
                return true;
            }
        }
        return false;
    }
    // 몬스터가 전부 플레그를 올렸는지 확인
    private bool CheckMonsterFlag()
    {
        if (SummonedMonster.Count > 0)
        {
            for (int index = 0; index < SummonedMonster.Count; index++)
            {
                if (SummonedMonster[index].GetComponent<Monster>().Flag)
                {
                    if (index == SummonedMonster.Count - 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
    // 몬스터 사망처리 함수
    private void MonsterDeathProcessing()
    {
        if (SummonedMonster.Count > 0)
        {
            for (int index = SummonedMonster.Count - 1; index >= 0; index--)
            {
                if (SummonedMonster[index].GetComponent<Monster>().Die)
                {
                    SummonedMonster.RemoveAt(index);
                }
            }
        }
    }
    // 몬스터 움직임 초기화
    private void ResetMonsterMovements()
    {
        if (SummonedMonster.Count > 0)
        {
            foreach (var monster in SummonedMonster)
            {
                monster.GetComponent<Monster>().MoveCount = 0;
                monster.GetComponent<Monster>().AttackCount = 0;
            }
        }
    }
    // 몬스터 플래그를 내림
    private void ResetMonsterFlags()
    {
        if (SummonedMonster.Count > 0)
        {
            for (int index = 0; index < SummonedMonster.Count; index++)
            {
                SummonedMonster[index].GetComponent<Monster>().Flag = false;
            }
        }
    }
    // 몬스터의 거리확인
    public void FindNearbyMonsters()
    {
        monsterDistances = new List<float>();
        if (SummonedMonster.Count > 0)
        {
            for (int index = 0; index < SummonedMonster.Count; index++)
            {
                monsterDistances.Add(Vector2.Distance(player.transform.position, SummonedMonster[index].transform.position));
            }
            InsertionSort();
            SummonedMonster[0].GetComponent<Monster>().Authority = true;
        }
    }
    // 플레이어에서 몬스터로
    public void FromPlayerToMonster()
    {
        Instance.playerState = State.Idle;
        changePlayerTurn = true;
    }
    // 플레이어와 가까운 몬스터 확인
    public void InsertionSort()
    {
        int j = 0;
        float key = 0;
        GameObject gameObject = null;
        for (int i = 1; i < SummonedMonster.Count; i++)
        {
            key = monsterDistances[i];
            gameObject = SummonedMonster[i].gameObject;
            for (j = i - 1; j >= 0; j--)
            {
                if (monsterDistances[j] > key)
                {
                    monsterDistances[j + 1] = monsterDistances[j];
                    SummonedMonster[j + 1] = SummonedMonster[j];
                }
                else
                {
                    break;
                }
            }

            monsterDistances[j + 1] = key;
            SummonedMonster[j + 1] = gameObject;
        }
    }
}

