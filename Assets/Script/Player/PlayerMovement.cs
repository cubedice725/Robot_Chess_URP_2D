using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class PlayerMovement : AStar
{
    private Animator animator;

    private Vector2 targetPosition;

    private bool updateMoveStart = true;
    private float xAxis = 0;
    private int count = 1;

    
    protected void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        
        
    }
    // 플레이어 움직임 관련 함수
    //----------------------------------------------------------
    public bool UpdateMove()
    {
        if (updateMoveStart)
        {
            PathFinding(
                Instance.PlayerPositionInt,
                new Vector3Int(Instance.hit.positionInt.x, Instance.hit.positionInt.y, 0),
                Vector3Int.zero,
                new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
            );
            targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
            updateMoveStart = false;
        }

        float distance = Vector2.Distance(transform.position, targetPosition);
        // 캐릭터가 최종적으로 움직이는 좌표의 거리, 좌표 오차는 여기서 수정
        if (distance > 0.01f)
        {
            RunAnimation(true, targetPosition.x - transform.position.x);
            // 캐릭터를 이동
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Instance.player.PlayerMoveSpeed * Time.deltaTime);
            return true;
        }
        else
        {
            // 도착하면 FinalNodeList가 남아 있는지 확인
            // count의 시작은 1이고, 시작하자 마자 1이 증가함으로 + 1을 함
            if (count + 1 < FinalNodeList.Count)
            {
                count++;
                targetPosition = new Vector2(FinalNodeList[count].x, FinalNodeList[count].y);
                return true;
            }
            else
            {
                RunAnimation(false);
                Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y] = (int)MapObject.player;
                count = 1;
                updateMoveStart = true;
                return false;
            }
        }
    }
    public bool UpdateMovePlaneCheck()
    {
        if (!Instance.playerTurn) return false;

        if (Instance.hit != null && Instance.hit.name.StartsWith("MovePlane"))
        {
            Instance.RemoveMovePlane();
            return true;
        }
        return false;
    }
    
    
    public void MoveReady()
    {
        // 실제 맵이랑 플레이어가 움직이는 임의 탐색 공간의 간격
        Vector3Int interval = new Vector3Int(Instance.PlayerPositionInt.x - Instance.player.MoveDistance, Instance.PlayerPositionInt.y - Instance.player.MoveDistance, 0);
        
        // 지름 계산
        int diameter = Instance.player.MoveDistance * 2 + 1;

        // 플레이어가 움직이는 공간을 먼저 추측하기 위해 이동거리를 기준으로 함
        for (int i = 0; i < diameter * diameter; i++)
        {
            // 플레이어의 공간 좌표
            int playerAreaX = (i / diameter);
            int playerAreaY = (i % diameter);

            // 실제 맵공간 좌표
            int mapAreaX = interval.x + playerAreaX;
            int mapAreaY = interval.y + playerAreaY;

            int distanceX = playerAreaX - Instance.player.MoveDistance;
            int distanceY = playerAreaY - Instance.player.MoveDistance;
            // 맵안 이고 플레이어가 아닌곳
            if ((mapAreaX > 0 && mapAreaY > 0 && mapAreaX < Instance.MapSizeX && mapAreaY < Instance.MapSizeY) && (playerAreaX != Instance.player.MoveDistance || playerAreaY != Instance.player.MoveDistance))
            {
                // 해당 공간에 벽이나 몬스터가 아닌곳
                if (Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.wall && Instance.Map2D[mapAreaX, mapAreaY] != (int)MapObject.moster)
                {
                    // 플레이어가 움직일수 있는 거리 안 즉 원안에 있는지
                    if (Mathf.FloorToInt(Mathf.Sqrt((distanceX * distanceX) + (distanceY * distanceY))) <= Instance.player.MoveDistance)
                    {
                        // 경로 탐색
                        PathFinding(
                            Instance.PlayerPositionInt,
                            new Vector3Int(mapAreaX, mapAreaY, 0),
                            Vector3Int.zero,
                            new Vector3Int(Instance.MapSizeX, Instance.MapSizeY, 0)
                        );
                        // 경로 탐색이 잘 되었는지, 이동 거리가 적절한지
                        if (FinalNodeList.Count > 1 && FinalNodeList.Count <= Instance.player.MoveDistance + 1)
                        {
                            Instance.SetMovePlane(new Vector2(mapAreaX, mapAreaY));
                        }
                    }
                }
            }
        }
    }

    // 애니메이션 관련 함수
    //----------------------------------------------------------
    
    public void LookMonsterAnimation(float target)
    {
        float direction = target - transform.position.x;

        if (direction == 0) return;
        if (xAxis == Mathf.Sign(direction)) return;

        xAxis = Mathf.Sign(direction);

        if (Mathf.Sign(direction) < 0)
        {
            // 왼쪽 방향
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // 오른쪽 방향
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void RunAnimation(bool run, float direction = 0)
    {
        animator.SetBool("run", run);
        if (direction == 0) return;
        if (xAxis == Mathf.Sign(direction)) return;

        xAxis = Mathf.Sign(direction);

        if (Mathf.Sign(direction) < 0)
        {
            // 왼쪽 방향
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // 오른쪽 방향
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
