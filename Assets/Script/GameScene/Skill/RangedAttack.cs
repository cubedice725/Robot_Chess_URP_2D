using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class RangedAttack : MonoBehaviour, IState
{
    Vector3Int CurrentMousPos = Vector3Int.zero;
    MyObject fighterPlaneShadow;
    MyObject fakeBullet;
    float time = 0;
    int numZ, numY = - 4;
    public bool start = false;
    public bool skillUse = false;

    bool mousePosUp, mousePosDown, mousePosLeft, mousePosRight, mousePosMiddle;
    public void Entry()
    {
        Instance.ButtonLock = true;
    }

    public void IStateUpdate()
    {
        Vector3 mousePos = Vector3.zero;
        if (!start)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        Vector3Int mousePosInt = new Vector3Int(
                (int)Mathf.Round(mousePos.x),
                (int)Mathf.Round(mousePos.y),
                -1
            );
        if (Instance.MyHit != null && Instance.MyHit.name.StartsWith("Selection"))
        {
            Instance.MyHit.name = "";
            start = true;
            numZ = mousePosInt.y + mousePosInt.x;
            fighterPlaneShadow = Instance.poolManager.SelectPool(PoolManager.Prefabs.FighterPlaneShadow).Get();
            fighterPlaneShadow.gameObject.transform.position = new Vector3Int(-100, -100, 0);
            fighterPlaneShadow.transform.position = new Vector3Int(-numY + numZ - 4, numY, 0);

            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.DamagedArea);
            Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);
            StartCoroutine(Shoot(mousePosInt));
        }
        if (start)
        {
            time += Time.deltaTime;
            fighterPlaneShadow.transform.Translate(0, 40f * Time.deltaTime, 0);

            if (time > 5f)
            {
                skillUse = true;
                start = false;
                time = 0;
            }
        }
        if (skillUse)
        {
            fighterPlaneShadow.Destroy();
            skillUse = false;
            Instance.playerState = State.Idle;
            GetComponent<MyObject>().Destroy();
        }
        if (CurrentMousPos == mousePosInt) return;

        CurrentMousPos = mousePosInt;
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.DamagedArea);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);

        AttackRange(1, CurrentMousPos, Vector2Int.zero, new Vector2Int(Instance.MapSizeX - 1, Instance.MapSizeY - 1));
    }
    public bool Exit()
    {
        Instance.summonedSkill = null;
        return false;
    }
    IEnumerator Shoot(Vector3Int mousePosInt)
    {
        yield return new WaitForSeconds(0.8f);
        int index = 0;
        float time = 0.02f;
        for (index = 0; index < 10; index++)
        {
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f) + 1, mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
            yield return new WaitForSeconds(time);
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f) - 1, 0));
            yield return new WaitForSeconds(time);
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
            yield return new WaitForSeconds(time);
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f) - 1, mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
            yield return new WaitForSeconds(time);
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f) + 1, 0));
            yield return new WaitForSeconds(time);
        }
        if (mousePosMiddle)
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x, mousePosInt.y, 0);
        }
        if (mousePosUp)
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position 
                = new Vector3Int(mousePosInt.x, mousePosInt.y + 1, 0);
        }
        if (mousePosRight)
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x + 1, mousePosInt.y, 0);
        }
        if (mousePosLeft)
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x - 1, mousePosInt.y, 0);
        }
        if (mousePosDown)
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                = new Vector3Int(mousePosInt.x, mousePosInt.y - 1, 0);
        }
        for (index = 0; index < 10; index++)
        {
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f) + 1, mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
            yield return new WaitForSeconds(time);
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f) - 1, 0));
            yield return new WaitForSeconds(time);
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
            yield return new WaitForSeconds(time);
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f) - 1, mousePosInt.y + Random.Range(-0.5f, 0.5f), 0));
            yield return new WaitForSeconds(time);
            SpawnFakeBullet(new Vector3(mousePosInt.x + Random.Range(-0.5f, 0.5f), mousePosInt.y + Random.Range(-0.5f, 0.5f) + 1, 0));
            yield return new WaitForSeconds(time);
        }
    }
    public void SpawnFakeBullet(Vector3 targetPos)
    {
        fakeBullet = Instance.poolManager.SelectPool(PoolManager.Prefabs.FakeBullet).Get();
        fakeBullet.GetComponent<FakeBullet>().targetPos = targetPos;
        fakeBullet.transform.position = new Vector3Int(-numY + numZ + 2, numY, 0);
        fakeBullet.transform.rotation = Quaternion.Euler(new Vector3(
            0, 0, TargetToAngle(targetPos)));
    }
    public float TargetToAngle(Vector3 target)
    {
        float angle;
        Vector3 direction = target - new Vector3Int(-numY + numZ + 2, numY, 0);
        direction.Normalize(); // 방향 벡터 정규화
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
    public void AttackRange(int size, Vector3Int targetPos, Vector2Int startPos, Vector2Int endPos)
    {
        bool downSide = targetPos.y > startPos.y;
        bool upSide = targetPos.y < endPos.y;
        bool leftSide = targetPos.x > startPos.x;
        bool rightSide = targetPos.x < endPos.x;

        if (downSide && upSide && rightSide && leftSide)
        {
            Instance.poolManager.SelectPool(PoolManager.Prefabs.Selection).Get().transform.position
                = new Vector3(targetPos.x, targetPos.y, -1);
            mousePosMiddle = false;
            if (Instance.Map2D[targetPos.x, targetPos.y] == (int)MapObject.moster || Instance.Map2D[targetPos.x, targetPos.y] == (int)MapObject.player)
            {
                Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                = new Vector3(targetPos.x, targetPos.y, 0);
                mousePosMiddle = true;
            }
        }
        for (int index = 1; index <= size; index++)
        {
            int downPos = targetPos.y - index;
            int upPos = targetPos.y + index;
            int leftPos = targetPos.x - index;
            int rightPos = targetPos.x + index;

            bool downPosSide = downPos > startPos.y && downPos < endPos.y;
            bool upPosSide = upPos > startPos.y && upPos < endPos.y;
            bool leftPosSide = leftPos > startPos.x && leftPos < endPos.x;
            bool rightPosSide = rightPos > startPos.x && rightPos < endPos.x;


            if (downPosSide && leftSide && rightSide)
            {
                if (Instance.Map2D[targetPos.x, downPos] == (int)MapObject.moster || Instance.Map2D[targetPos.x, downPos] == (int)MapObject.player)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                        = new Vector3(targetPos.x, downPos, -1);
                    mousePosDown = true;
                }
                else
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.UnSelection).Get().transform.position
                        = new Vector3(targetPos.x, downPos, -1);
                    mousePosDown = false;
                }
            }

            if (upPosSide && leftSide && rightSide)
            {
                if (Instance.Map2D[targetPos.x, upPos] == (int)MapObject.moster || Instance.Map2D[targetPos.x, upPos] == (int)MapObject.player)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                        = new Vector3(targetPos.x, upPos, -1);
                    mousePosUp = true;
                }
                else
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.UnSelection).Get().transform.position
                        = new Vector3(targetPos.x, upPos, -1);
                    mousePosUp = false;
                }
            }

            if (leftPosSide && upSide && downSide)
            {
                if (Instance.Map2D[leftPos, targetPos.y] == (int)MapObject.moster || Instance.Map2D[leftPos, targetPos.y] == (int)MapObject.player)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                        = new Vector3(leftPos, targetPos.y, -1);
                    mousePosLeft = true;
                }
                else
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.UnSelection).Get().transform.position
                        = new Vector3(targetPos.x - index, targetPos.y, -1);
                    mousePosLeft = false;
                }
            }

            if (rightPosSide && upSide && downSide)
            {
                if (Instance.Map2D[rightPos, targetPos.y] == (int)MapObject.moster || Instance.Map2D[rightPos, targetPos.y] == (int)MapObject.player)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.DamagedArea).Get().transform.position
                        = new Vector3(rightPos, targetPos.y, -1);
                    mousePosRight = true;
                }
                else
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.UnSelection).Get().transform.position
                        = new Vector3(rightPos, targetPos.y, -1);
                    mousePosRight = false;
                }
            }
        }
    }

    
}
