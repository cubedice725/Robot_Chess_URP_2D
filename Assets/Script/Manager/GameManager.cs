using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.EditorTools;
using UnityEngine;

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

    private Stage1 stage1;

    public State playerState = State.Idle;
    public IState skillState;

    private int monsterFlagCount = 0;
    private int monsterAuthorityCount = 0;

    public bool monsterTurn = false;
    public bool playerTurn = true;
    public bool changePlayerTurn = false;
    public bool changeMonsterTurn = false;

    public Player player { get; set; }
    public PoolManager poolManager { get; set; }
    public int MapSizeX { get; set; } = 11;
    public int MapSizeY { get; set; } = 15;
    public int[,] Map2D { get; set; }

    // �ʵ忡 ������Ʈ ���翩�� Ȯ��
    public bool MyObjectActivate { get; set; } = false;
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

        player = FindObjectOfType<Player>();
        poolManager = GetComponent<PoolManager>();
        stage1 = GetComponent<Stage1>();
    }
    private void Start()
    {
        stage1.Opening();
    }
    private void Update()
    {
        // ������ ���Ͱ� �����Ѵٸ�
        if (spawnMonsters.Count > 0)
        {
            // ���Ͱ� �����̴� ������ ���ٸ� ���� ���Ͱ� �������� �ȴٰ� �Ǵ�
            if (!GetSpawnMonsterComponent(monsterAuthorityCount).Authority)
            {
                // �������� ������ �ѱ�
                if (monsterAuthorityCount < spawnMonsters.Count - 1)
                {
                    monsterAuthorityCount++;
                    GetSpawnMonsterComponent(monsterAuthorityCount).Authority = true;
                }
                // �����̴� ������ ������ ���Ϳ� �� Ȯ���� ������
                else
                {
                    //print("���� ���� �Ѿ," + monsterAuthorityCount);
                    // �������� ������ ����� ���͸� Ȯ��
                    if (GetSpawnMonsterComponent(monsterFlagCount).Flag)
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
                                if (GetSpawnMonsterComponent(count).Die)
                                {
                                    // Monster.cs�� Update()�ּ��� �����ϸ� ����.
                                    GetSpawnMonsterComponent(count).Authority = true;
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
                                    GetSpawnMonsterComponent(monsterFlagCount).Flag = false;
                                }
                                GetSpawnMonsterComponent(0).Authority = true;
                            }
                            
                            monsterFlagCount = 0;
                            monsterAuthorityCount = 0;
                            FromMonsterToPlayer();
                        }
                    }
                    
                }
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
        //if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ��
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //    if(hit.collider != null)
        //    {
        //        Debug.Log("Ŭ���� ������Ʈ �̸�: " + hit.collider.gameObject.name);
        //    }
        //}
    }
    public Monster GetSpawnMonsterComponent(int index)
    {
        return spawnMonsters[index].GetComponent<Monster>();
    }
    public void FromPlayerToMonster()
    {
        changePlayerTurn = true;
    }
    public void FromMonsterToPlayer()
    {
        changeMonsterTurn = true;
    }
}

