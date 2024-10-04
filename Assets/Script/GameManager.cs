using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
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
    public int MapSizeX { get; set; } = 11;
    public int MapSizeY { get; set; } = 15;
    public int[,] Map2D { get; set; }

    public List<GameObject> spawnMonsters = new List<GameObject>();
    public IState skillState;

    private int count = 0;

    public bool monsterTurn = false;
    public bool playerTurn = true;
    public State playerState = State.Idle;

    private Stage1 stage1;

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
        stage1 = GetComponent<Stage1>();
        

    }
    private void Start()
    {
        stage1.Opening();
    }
    public void FromPlayerToMonster()
    {
        Instance.monsterTurn = true;
        Instance.playerTurn = false;
    }
    public void FromMonsterToPlayer()
    {
        monsterTurn = false;
        playerTurn = true;
    }

    private void Update()
    {
        if (spawnMonsters.Count > 0)
        {
            if (spawnMonsters[count].GetComponent<Monster>().flag)
            {
                if (count < spawnMonsters.Count - 1)
                {
                    count++;
                }
                else
                {
                    for (count = 0; count < spawnMonsters.Count; count++)
                    {
                        spawnMonsters[count].GetComponent<Monster>().flag = false;
                    }
                    count = 0;
                    FromMonsterToPlayer();
                }
            }
        }
        //else if (spawnMonsters.Count == 0)
        //{
        //    FromMonsterToPlayer();
        //}
       
        //if(Input.GetMouseButtonDown(1))
        //{
        //    monsterTurn = false;
        //}
        //if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ��
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //    if(hit.collider != null)
        //    {
        //        Debug.Log("Ŭ���� ������Ʈ �̸�: " + hit.collider.gameObject.name);
        //    }
        //}
    }
}

