using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Node
{
    public Node ParentNode;

    public bool isWall;
    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }
    public int F { get { return G + H; } }
}

public abstract class AStar : MonoBehaviour
{
    [SerializeField]
    protected List<Node> FinalNodeList;
    [SerializeField]
    protected List<Node> OpenList, ClosedList;
    protected Node[,] NodeArray;
    protected Node StartNode, TargetNode, CurNode;

    protected virtual bool AllowDiagonal { get; set; } = true;
    protected bool dontCrossCorner = true;
    protected int sizeX, sizeY;
    protected Vector3Int bottomLeft, topRight, startPos, targetPos;

    protected void PathFinding(Vector3Int start, Vector3Int target, Vector3Int mapMini, Vector3Int mapMax, List<GameManager.MapObject> wallType)
    {
        startPos = start;
        
        targetPos = target;

        bottomLeft = mapMini;

        topRight = mapMax;

        // NodeArray의 크기 정해주고, isWall, x, z 대입
        sizeX = topRight.x - bottomLeft.x;
        sizeY = topRight.y - bottomLeft.y;
        NodeArray = new Node[sizeX, sizeY];

        for (int indexI = 0; indexI < sizeX * sizeY; indexI++)
        {
            bool isWall = false;
            for (int indexJ = 0; indexJ < wallType.Count; indexJ++)
            {
                if ((int)wallType[indexJ] == GameManager.Instance.Map2D[indexI / sizeY, indexI % sizeY])
                {
                    isWall = true;
                    break;
                }
            }
            NodeArray[indexI / sizeY, indexI % sizeY] = new Node(isWall, (indexI / sizeY) + bottomLeft.x, (indexI % sizeY) + bottomLeft.y);
        }

        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();


        while (OpenList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 마지막
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                return;
            }


            // ↗↖↙↘
            if (AllowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    private void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX > bottomLeft.x && checkX < topRight.x && checkY > bottomLeft.y && checkY < topRight.y && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // 대각선 허용시, 벽 사이로 통과 안됨
            if (AllowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            
            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }
}

