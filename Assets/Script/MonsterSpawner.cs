using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;
using static PoolManager;

public class MonsterSpawner : MonoBehaviour
{
    public Vector3Int startPosition;

    private Vector2Int size;
    private List<MyObject> points = new List<MyObject>();

    private int unspondedMonsters = 0;
    private bool start = false;
    private void Awake()
    {
        size.x = Instance.MapSizeX - 2;
        size.y = Instance.MapSizeY - 2;
    }
    void Start()
    {
        CreateBorder();
        SpawnMonster(1);
    }
    void AddPoint(int x, int y)
    {
        points.Add(Instance.poolManager.SelectPool(Prefabs.Point).Get());
        points[points.Count - 1].transform.position = startPosition + new Vector3Int(x, y, 0);
        points[points.Count - 1].transform.parent = transform;
    }

    void CreateBorder()
    {
        // ���ʰ� ������ �׵θ�
        for (int y = 0; y < size.y; y++)
        {
            AddPoint(0, y);
            AddPoint(size.x - 1, y);
        }

        // ���ʰ� �Ʒ��� �׵θ� (�𼭸� �ߺ� ����)
        for (int x = 1; x < size.x - 1; x++)
        {
            AddPoint(x, 0);
            AddPoint(x, size.y - 1);
        }
    }
    private void Update()
    {
        if (Instance.monsterTurn)
        {
            start = true;
        }
        if(Instance.GameTurnCount % 1 == 0 && Instance.playerTurn && Instance.GameTurnCount != 0 && start)
        {
            //SpawnMonster(5);
            //SpawnMonster(0);
            start = false; 
        }
    }
    private void SpawnMonster(int numMonstersSummon)
    {
        int unavailableNumberCount = 0;
        bool[] verifiedNumber = new bool[points.Count];
        for (int index = 0; index < points.Count; index++)
        {
            if (Instance.Map2D[(int)Mathf.Round(points[index].transform.position.x), (int)Mathf.Round(points[index].transform.position.y)] != (int)MapObject.player
                && Instance.Map2D[(int)Mathf.Round(points[index].transform.position.x), (int)Mathf.Round(points[index].transform.position.y)] != (int)MapObject.moster)
            {
                verifiedNumber[index] = true;
            }
            else
            {
                unavailableNumberCount++;
            }
            //print(verifiedNumber[20]);

        }
        if (numMonstersSummon == 0)
        {
            if(unavailableNumberCount == points.Count)
            {
                // print("����");
                unspondedMonsters += unavailableNumberCount - points.Count;
               // print("���ȯ" + unspondedMonsters);
                numMonstersSummon = points.Count - unavailableNumberCount;
            }
        }
        if (points.Count - unavailableNumberCount <= 0)
        {
            // print("��ȯ ���Ͽ� �� ��");
            unspondedMonsters += numMonstersSummon;
            // print("��ȯ���� ����" + unspondedMonsters);

            return;
        }
        for (int i = 0; i < numMonstersSummon;)
        {
            if (unavailableNumberCount == points.Count)
            {
                // print("��ȯ�� ����" + i);
                // print("���� ����" + (numMonstersSummon - i));
                unspondedMonsters += numMonstersSummon - i;
                // print("��ȯ���� ����" + unspondedMonsters);
                return;
            }
            int RandomNum = Random.Range(0, points.Count);
            if (verifiedNumber[RandomNum])
            {
                MyObject monsterObject = Instance.poolManager.SelectPool(Prefabs.Robot).Get();
                monsterObject.transform.GetComponent<Monster>().Initialize();
                Instance.SummonedMonster.Add(monsterObject.gameObject);
                monsterObject.transform.position = points[RandomNum].transform.position;
                verifiedNumber[RandomNum] = false;
                unavailableNumberCount++;
                i++;
            }
        }
    }
}
