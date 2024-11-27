using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static PoolManager;

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
    // ���ʹ� Ȱ��(MovingState, SkillState)�� ���� ������ Authority = false�� ������ �˷�����
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

    private int SummonedMonsterAuthorityCount = 0;

    public bool playerSkillUse = false;
    public bool monsterTurn = false;
    public bool playerTurn = true;
    public bool changePlayerTurn = false;
    public bool changeMonsterTurn = false;
    public int monsterTurnCount = 0;
    public Player player { get; set; }
    public PoolManager poolManager { get; set; }
    public MonsterSpawner monsterSpawner { get; set; }
    public Hit hit;
    public bool ButtonLock { get; set; } = false;

    public int MapSizeX { get; set; } = 12;
    public int MapSizeY { get; set; } = 12;
    public int[,] Map2D { get; set; }
    public int GameScore { get; set; } = 0;
    public int GameTurnCount { get; set; } = 0;
    // �ʵ忡 ������Ʈ ���翩�� Ȯ��
    public bool MyObjectActivate  = false;
    public Vector3Int PlayerPositionInt { get; set; }
    public static GameManager Instance
    {
        get
        {
            // �ν��Ͻ��� ���� ��쿡 �����Ϸ� �ϸ� �ν��Ͻ��� �Ҵ����ش�.
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

        playerSkillUse = false;
        monsterTurn = false;
        playerTurn = true;
        changePlayerTurn = false;
        changeMonsterTurn = false;
        monsterTurnCount = 0;
        GameScore = 0;
        GameTurnCount = 0;
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
            // �ν��Ͻ��� �����ϴ� ��� ���λ���� �ν��Ͻ��� �����Ѵ�.
            Destroy(gameObject);
        }
        // �Ʒ��� �Լ��� ����Ͽ� ���� ��ȯ�Ǵ��� ����Ǿ��� �ν��Ͻ��� �ı����� �ʴ´�.
        DontDestroyOnLoad(gameObject);

        // �ʿ� ���� ������ ����
        Map2D = new int[MapSizeX, MapSizeY];

        for (int i = 0; i < MapSizeX * MapSizeY; i++)
        {
            Map2D[i / MapSizeY, i % MapSizeY] = (int)MapObject.noting;
        }
        hit = new Hit("", Vector3Int.zero);
        player = FindObjectOfType<Player>();
        poolManager = FindObjectOfType<PoolManager>();
        monsterSpawner = FindObjectOfType<MonsterSpawner>();
    }
    private void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        if (poolManager == null)
        {
            poolManager = FindObjectOfType<PoolManager>();
        }

        PlayerPositionInt = new Vector3Int(
            (int)Mathf.Round(player.transform.position.x),
            (int)Mathf.Round(player.transform.position.y),
            (int)Mathf.Round(transform.position.z)
        );
        
        // ������ �������� ��Ģ�� ����ϴ� �˰���
        if (monsterTurn && SummonedMonster.Count > 0)
        {
            // ���Ͱ� N�� �ൿ �Ҽ� �ֵ��� ��
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
                GameTurnCount++;
                GameScore += 10;
                monsterTurnCount = 0;
                SummonedMonsterAuthorityCount = 0;
                ResetMonsterMovements();
                FromMonsterToPlayer();
            }
        }
        else if (SummonedMonster.Count == 0)
        {
            FromMonsterToPlayer();
        }

        if (changePlayerTurn && !MyObjectActivate)
        {
            ButtonLock = true;
            ResetMonsterMovements();
            MonsterDeathProcessing();
            ResetMonsterFlags();
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

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hits.Length > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                print(hits[0].transform.name);
                
            }
            try{
                hit.name = hits[0].collider.name;
                hit.positionInt = new Vector3Int(
                    (int)Mathf.Round(hits[0].collider.transform.position.x),
                    (int)Mathf.Round(hits[0].collider.transform.position.y),
                    (int)Mathf.Round(hits[0].collider.transform.position.z)
                );
            }
            catch
            {
                hit.name = "";
            }
        }
    }
    // ���� ���� Ȯ��
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
    // ���Ͱ� ���� �÷��׸� �÷ȴ��� Ȯ��
    private bool CheckMonsterFlag()
    {
        
        if (SummonedMonster.Count > 0)
        {
            for (int index = 0; index < SummonedMonster.Count; index++)
            {
                if (SummonedMonster[index].GetComponent<Monster>().Flag)
                {
                    if (index + 1 == SummonedMonster.Count)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    // ���� ���ó�� �Լ�
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
    // ���� ������ �ʱ�ȭ
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
    // ���� �÷��׸� ����
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
    //������ �Ÿ�Ȯ��
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
 
    public void FromPlayerToMonster()
    {
        Instance.playerState = State.Idle;
        changePlayerTurn = true;
    }
    public void FromMonsterToPlayer()
    {
        changeMonsterTurn = true;
    }

    // �÷��̾�� ����� ���� Ȯ��
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

