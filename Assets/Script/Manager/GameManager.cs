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

    public List<GameObject> spawnMonsters = new List<GameObject>();
    private Stage1 stage1;

    public State playerState = State.Idle;
    public IState skillState;

    private int monsterFlagCount = 0;
    private int monsterAuthorityCount = 0;

    public bool monsterTurn = false;
    public bool playerTurn = true;
    public bool changePlayerTurn = false;
    public bool changeMonsterTurn = false;


    public int MapSizeX { get; set; } = 11;
    public int MapSizeY { get; set; } = 15;
    public int[,] Map2D { get; set; }

    // 필드에 오브젝트 존재여부 확인
    public bool MyObjectActivate { get; set; } = false;
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
        stage1 = GetComponent<Stage1>();
    }
    private void Start()
    {
        stage1.Opening();
    }
    private void Update()
    {
        if (spawnMonsters.Count > 0)
        {
            if (!spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority)
            {
                if (monsterAuthorityCount < spawnMonsters.Count - 1)
                {
                    monsterAuthorityCount++;
                    spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority = true;
                }
                else
                {
                    spawnMonsters[0].GetComponent<Monster>().Authority = true;
                    monsterAuthorityCount = 0;
                }
            }
            if (spawnMonsters[monsterFlagCount].GetComponent<Monster>().Flag)
            {
                if (monsterFlagCount < spawnMonsters.Count - 1)
                {
                    monsterFlagCount++;
                }
                else
                {
                    for (monsterFlagCount = 0; monsterFlagCount < spawnMonsters.Count; monsterFlagCount++)
                    {
                        spawnMonsters[monsterFlagCount].GetComponent<Monster>().Flag = false;
                    }
                    monsterFlagCount = 0;
                    FromMonsterToPlayer();
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
        //if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //    if(hit.collider != null)
        //    {
        //        Debug.Log("클릭한 오브젝트 이름: " + hit.collider.gameObject.name);
        //    }
        //}
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

