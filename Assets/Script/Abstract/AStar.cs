using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


[Serializable]
public class Node
{
    public Node ParentNode;

    public bool isWall;
    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
    public int x, y, G, H;
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }
    public int F { get { return G + H; } }
}

public class AStar : MonoBehaviour
{
    [SerializeField]
    protected List<Node> FinalNodeList;
    protected List<Node> OpenList, ClosedList;
    protected Player player;
    protected Node[,] NodeArray;
    protected Node StartNode, TargetNode, CurNode;

    protected bool allowDiagonal = true;
    protected bool dontCrossCorner = true;
    protected int sizeX, sizeY;
    protected Vector3Int bottomLeft, topRight, startPos, targetPos;

    protected virtual void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    protected virtual void SetPathFinding()
    {
        // NodeArray�� ũ�� �����ְ�, isWall, x, z ����
        sizeX = topRight.x - bottomLeft.x;
        sizeY = topRight.y - bottomLeft.y;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX * sizeY; i++)
        {
            bool isWall = false;
            if ((int)GameManager.MapObject.wall == GameManager.Instance.Map2D[i / sizeY, i % sizeY] || (int)GameManager.MapObject.moster == GameManager.Instance.Map2D[i / sizeY, i % sizeY])
            {
                isWall = true;
            }
            NodeArray[i / sizeY, i % sizeY] = new Node(isWall, (i / sizeY) + bottomLeft.x, (i % sizeY) + bottomLeft.y);
        }

        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.z - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.z - bottomLeft.y];
    }


    protected void PathFinding(Vector3Int start, Vector3Int target, Vector3Int mapMini, Vector3Int mapMax)
    {
        startPos = start;
        
        targetPos = target;

        bottomLeft = mapMini;

        topRight = mapMax;

        SetPathFinding();

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();


        while (OpenList.Count > 0)
        {
            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                {
                    CurNode = OpenList[i];
                }
            }

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            // ������
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

            // �֢آע�
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // �� �� �� ��
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    protected void OpenListAdd(int checkX, int checkZ)
    {
        if (OpenListAddCondition(checkX,checkZ))
        {
            // �밢�� ����, �� ���̷� ��� �ȵ�
            if (allowDiagonal)
            {
                if (NodeArray[CurNode.x - bottomLeft.x, checkZ - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall)
                {
                    return;
                }
            }

            // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
            if (dontCrossCorner)
            {
                if (NodeArray[CurNode.x - bottomLeft.x, checkZ - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall)
                {
                    return;
                }
            }

            // �̿���忡 �ְ�, ������ 10, �밢���� 14���
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkZ == 0 ? 10 : 14);

            // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }
    protected virtual bool OpenListAddCondition(int checkX, int checkZ)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= bottomLeft.x && checkX < topRight.x && checkZ >= bottomLeft.y && checkZ < topRight.y)
        {
            if (!NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.y]))
            {
                return true;
            }
        }
        return false;
    }
}

