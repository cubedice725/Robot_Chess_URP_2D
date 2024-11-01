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
    public bool MyObjectActivate { get; set; } = false;
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
        hit = new Hit("",Vector3Int.zero);
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

        player = FindObjectOfType<Player>();
        poolManager = GetComponent<PoolManager>();
    }

    private void Update()
    {
        // �ݿø��� ���� ������ �����̰� �� �� x��ǥ�� 1.999�϶� Ÿ���ɽ�Ʈ�� 1�� �ȴ�.
        PlayerPositionInt = new Vector3Int((int)Mathf.Round(player.transform.position.x), (int)Mathf.Round(player.transform.position.y), (int)Mathf.Round(transform.position.z));
        
        // ������ ���Ͱ� �����ϰ� ���� ���̰� ���Ͱ� �����̴� ������ ���ٸ� ���� ���Ͱ� �������� �ȴٰ� �Ǵ�
        if (spawnMonsters.Count > 0 && monsterTurn && !spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority)
        {
            
            if (monsterTurnCount < 2)
            {
                // �������� ������ �ѱ�
                if (monsterAuthorityCount < spawnMonsters.Count - 1)
                {
                    monsterAuthorityCount++;
                    spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority = true;
                }
                // �����̴� ������ ������ ���Ϳ� �� Ȯ���� ������
                else
                {
                    //print("���� ���� �Ѿ," + monsterAuthorityCount);
                    // �������� ������ ����� ���͸� Ȯ��
                    if (spawnMonsters[monsterFlagCount].GetComponent<Monster>().Flag)
                    {
                        // �������� Ȯ��
                        if (monsterFlagCount < spawnMonsters.Count - 1)
                        {
                            monsterFlagCount++;
                        }
                        else
                        {
                            //print("�÷��� ALL TRUE," + monsterFlagCount);
                            // ����� ���͸� Ȯ���ϰ� ó��, ���ݺ����� Remove�� ���� count�� �����߻��� ����
                            for (int count = spawnMonsters.Count - 1; count >= 0; count--)
                            {
                                if (spawnMonsters[count].GetComponent<Monster>().Die)
                                {
                                    // Monster.cs�� Update()�ּ��� �����ϸ� ����.
                                    spawnMonsters[count].GetComponent<Monster>().Authority = true;
                                    deadMonsters.Add(spawnMonsters[count]);
                                    spawnMonsters.RemoveAt(count);
                                    //print("���ó�� �Ϸ�," + count);
                                }
                            }
                            // ����� ���͸� ó������ spawnMonsters���� ���Ͱ� ������ ����
                            if (spawnMonsters.Count > 0)
                            {
                                // ����� ���� Flag ������
                                for (monsterFlagCount = 0; monsterFlagCount < spawnMonsters.Count; monsterFlagCount++)
                                {
                                    spawnMonsters[monsterFlagCount].GetComponent<Monster>().Flag = false;
                                }
                            }
                            monsterFlagCount = 0;
                            monsterAuthorityCount = 0;
                            monsterTurnCount++;
                            FindNearbyMonsters();
                        }
                    }
                }
            }
            else
            {
                // ������ �ൿ ī���͸� �ٽ� �ʱ�ȭ
                for (int monsterMovementCount = 0; monsterMovementCount < spawnMonsters.Count; monsterMovementCount++)
                {
                    spawnMonsters[monsterMovementCount].GetComponent<Monster>().MoveCount = 0;
                    spawnMonsters[monsterMovementCount].GetComponent<Monster>().AttackCount = 0;
                }
                GameturnCount++;
                GameScore += 10;
                monsterTurnCount = 0;
                FromMonsterToPlayer();
            }

        }
        else if (spawnMonsters.Count == 0)
        {
            FromMonsterToPlayer();
        }
        if (changePlayerTurn)
        {
            if (!MyObjectActivate)
            {
                FindNearbyMonsters();
                changePlayerTurn = false;
                monsterTurn = true;
                playerTurn = false;
            }
        }
        if (changeMonsterTurn)
        {
            if (!MyObjectActivate)
            {
                changeMonsterTurn = false;
                monsterTurn = false;
                playerTurn = true;
            }
        }
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ��
        {
            RaycastHit2D _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (_hit.collider != null)
            {
                hit.name = _hit.collider.name;
                hit.positionInt = new Vector3Int(
                    (int)Mathf.Round(_hit.collider.transform.position.x), 
                    (int)Mathf.Round(_hit.collider.transform.position.y), 
                    (int)Mathf.Round(_hit.collider.transform.position.z)
                    );
            }
        }
    }
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

    // �÷��̾� �� ���� �Լ�
    //----------------------------------------------------------
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

    // ��ų �� ���� �Լ�
    //----------------------------------------------------------
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

