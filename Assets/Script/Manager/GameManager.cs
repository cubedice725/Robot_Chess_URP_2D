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
        UpdatePlayerPosition();

        // ������ �������� ��Ģ�� ����ϴ� �˰���
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

    // ������ ���� �����ϴ� �Լ�
    private void HandleMonsterTurn()
    {
        // ���Ͱ� N�� �ൿ �Ҽ� �ֵ��� ��
        if (monsterTurnCount < 2)
        {
            TransferAuthorityToNextMonster();
        }
        // ������ �������� ������ �÷��̾����� �ѱ���� �ܰ�
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

    // ������ ������ Ȯ���ϴ� �Լ�
    private void TransferAuthorityToNextMonster()
    {
        // ������ ������ Ȯ��
        if (monsterAuthorityCount < spawnMonsters.Count - 1)
        {
            monsterAuthorityCount++;
            spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority = true;
        }
        // ������ ������ ������ �ൿ�� ����� Ȯ����
        else
        {
            CheckMonsterFlagsAndHandleDeaths();
        }
    }

    // �ൿ�� ������ ������ ����� �˼� ���⿡ �ൿ�� Ȯ�� �� ���� ���ó��
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

    // ���� ���ó�� �Լ�
    private void HandleDeadMonsters()
    {
        // ����� ���͸� Ȯ���ϰ� ó��, ���ݺ����� Remove�� ���� count�� �����߻��� ����
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

    // �÷��̾�� ����� ���� Ȯ��
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
        // RemoveAt���� ���� 0��°�� ������ ��� ������
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
        // RemoveAt���� ���� 0��°�� ������ ��� ������
        while (selectionList.Count > 0)
        {
            selectionList[0].Destroy();
            selectionList.RemoveAt(0);
        }
    }
}

