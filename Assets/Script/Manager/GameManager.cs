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
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        // �ν��Ͻ��� �����ϴ� ��� ���λ���� �ν��Ͻ��� �����Ѵ�.
        else if (_instance != this)
        {
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
        poolManager = GetComponent<PoolManager>();
    }
    private void Update()
    {
        PlayerPositionInt = new Vector3Int(
            (int)Mathf.Round(player.transform.position.x),
            (int)Mathf.Round(player.transform.position.y),
            (int)Mathf.Round(transform.position.z)
        );

        // ������ �������� ��Ģ�� ����ϴ� �˰���
        if (monsterTurn
            && poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count > 0
            && !poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][monsterAuthorityCount].GetComponent<Monster>().Authority)
        {
            // ���Ͱ� N�� �ൿ �Ҽ� �ֵ��� ��
            if (monsterTurnCount < 2)
            {
                // ������ ������ Ȯ��
                if (monsterAuthorityCount < poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count - 1)
                {
                    monsterAuthorityCount++;
                    poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][monsterAuthorityCount].GetComponent<Monster>().Authority = true;
                }
                // ������ ������ ������ �ൿ�� ����� Ȯ����
                else
                {
                    // �ൿ�� ������ ������ ����� �˼� ���⿡ �ൿ�� Ȯ�� �� ���� ���ó��
                    if (poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][monsterFlagCount].GetComponent<Monster>().Flag)
                    {
                        if (monsterFlagCount < poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count - 1)
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
            // ������ �������� ������ �÷��̾����� �ѱ���� �ܰ�
            else
            {
                ResetMonsterMovements();
                GameTurnCount++;
                GameScore += 10;
                monsterTurnCount = 0;
                FromMonsterToPlayer();
            }
        }
        else if (poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count == 0)
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

    // ���� ������ �ʱ�ȭ
    private void ResetMonsterMovements()
    {
        foreach (var monster in poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot])
        {
            monster.GetComponent<Monster>().MoveCount = 0;
            monster.GetComponent<Monster>().AttackCount = 0;
        }
    }


    // ���� ���ó�� �Լ�
    private void HandleDeadMonsters()
    {
        for (int count = poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count; count < 0; count++)
        {
            if (poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][count].GetComponent<Monster>().Die)
            {
                poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][count].GetComponent<Monster>().Authority = true;
            }
        }
    }

    // ���� �÷��׸� ����
    private void ResetMonsterFlags()
    {
        if (poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count > 0)
        {
            for (monsterFlagCount = 0; monsterFlagCount < poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count; monsterFlagCount++)
            {
                poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][monsterFlagCount].GetComponent<Monster>().Flag = false;
            }
        }

        monsterFlagCount = 0;
        monsterAuthorityCount = 0;
        monsterTurnCount++;
    }

    //������ �Ÿ�Ȯ��
    public void FindNearbyMonsters()
    {
        monsterDistances = new List<float>();
        for (int index = 0; index < poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count; index++)
        {
            monsterDistances.Add(Vector2.Distance(player.transform.position, poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][index].transform.position));
        }
        InsertionSort();
        poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][0].GetComponent<Monster>().Authority = true;
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

    // �÷��̾�� ����� ���� Ȯ��
    
    public void InsertionSort()
    {
        int j = 0;
        float key = 0;
        MyObject gameObject = null;
        for (int i = 1; i < poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot].Count; i++)
        {
            key = monsterDistances[i];
            gameObject = poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][i];
            for (j = i - 1; j >= 0; j--)
            {
                if (monsterDistances[j] > key)
                {
                    monsterDistances[j + 1] = monsterDistances[j];
                    poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][j + 1] = poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][j];
                }
                else
                {
                    break;
                }
            }

            monsterDistances[j + 1] = key;
            poolManager.MyObjectLists[(int)PoolManager.Prefabs.Robot][j + 1] = gameObject;
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

