using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Unity.Burst.CompilerServices;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UIElements;

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

    public bool monsterTurn = false;
    public bool playerTurn = true;
    public bool changePlayerTurn = false;
    public bool changeMonsterTurn = false;

    public Player player { get; set; }
    public PoolManager poolManager { get; set; }
    public int MapSizeX { get; set; } = 12;
    public int MapSizeY { get; set; } = 12;
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

        player = FindObjectOfType<Player>();
        poolManager = GetComponent<PoolManager>();
    }
    
    private void Update()
    {
        // 스폰된 몬스터가 존재한다면
        if (spawnMonsters.Count > 0 && monsterTurn)
        {
            // 몬스터가 움직이는 권한이 없다면 다음 몬스터가 움직여도 된다고 판단
            if (!spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority)
            {
                // 다음으로 권한을 넘김
                if (monsterAuthorityCount < spawnMonsters.Count - 1)
                {
                    monsterAuthorityCount++;
                    spawnMonsters[monsterAuthorityCount].GetComponent<Monster>().Authority = true;
                }
                // 움직이는 권한을 스폰된 몬스터에 다 확인이 끝나면
                else
                {
                    //print("권한 전부 넘어감," + monsterAuthorityCount);
                    // 움직임이 완전히 종료된 몬스터를 확인
                    if (spawnMonsters[monsterFlagCount].GetComponent<Monster>().Flag)
                    {
                        // 다음으로 확인
                        if (monsterFlagCount < spawnMonsters.Count - 1)
                        {
                            monsterFlagCount++;
                        }
                        else
                        {
                            //print("플래그 ALL TRUE," + monsterFlagCount);
                            // 사망한 몬스터를 확인하고 처리, 역반복문은 Remove로 인해 count의 오차발생을 방지
                            for (int count = spawnMonsters.Count - 1; count >= 0; count--)
                            {
                                if (spawnMonsters[count].GetComponent<Monster>().Die)
                                {
                                    // Monster.cs에 Update()주석을 참고하면 좋다.
                                    spawnMonsters[count].GetComponent<Monster>().Authority = true;
                                    deadMonsters.Add(spawnMonsters[count]);
                                    spawnMonsters.RemoveAt(count);
                                    //print("사망처리 완료," + count);
                                }
                            }
                            // 사망한 몬스터를 처리한후 spawnMonsters에는 몬스터가 없을수 있음
                            if (spawnMonsters.Count > 0)
                            {
                                // 사망한 몬스터 Flag 내리기
                                for (monsterFlagCount = 0; monsterFlagCount < spawnMonsters.Count; monsterFlagCount++)
                                {
                                    spawnMonsters[monsterFlagCount].GetComponent<Monster>().Flag = false;
                                }
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
                monsterDistances = new List<float>();
                for (int index = 0; index < spawnMonsters.Count; index++)
                {
                    monsterDistances.Add(Vector2.Distance(player.transform.position, spawnMonsters[index].transform.position));
                }
                InsertionSort();
                spawnMonsters[0].GetComponent<Monster>().Authority = true;

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
}

