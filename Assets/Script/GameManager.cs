using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public enum PlayerState
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
    public Skill skillState;

    private Stage1 stage1;
    private int count = 0;

    private bool MapCheck = true;
    public bool turnStart = false;
    public bool turnEnd = false;

    public bool monsterTurn = false;
    public bool playerTurn = true;
    public PlayerState playerState = PlayerState.Idle;

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
        Map2D = new int[MapSizeX, MapSizeY];

        for (int i = 0; i < MapSizeX * MapSizeY; i++)
        {
            Map2D[i / MapSizeY, i % MapSizeY] = (int)MapObject.noting;
        }
        stage1 = GetComponent<Stage1>();
    }

    public void FromPlayerToMonster()
    {
        Instance.monsterTurn = true;
        Instance.playerTurn = false;
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
                    monsterTurn = false;
                    playerTurn = true;
                }
            }
        }
        
        if (turnStart)
        {
            turnStart = false;
            turnEnd = true;
        }
        if(Input.GetMouseButtonDown(1))
        {
            monsterTurn = false;
        }
        if (turnEnd && Instance.playerState == PlayerState.Idle)
        {
            turnEnd = false;
        }
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Debug.Log("클릭한 오브젝트 이름: " + hit.collider.gameObject.name);
        }
    }
}

