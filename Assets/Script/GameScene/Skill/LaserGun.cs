using UnityEngine;
using static GameManager;
public class LaserGun : Skill, IState
{
    // .03 .15

    bool skillUse = false;
    bool start = false;
    bool ready = true;
    Vector3 targetPos;
    Vector3Int mousePos;
    GameObject laserPoint;
    Vector3Int laserPointPos;
    public override int UsageLimit { get => 1; set { } }

    public void Entry()
    {
        laserPoint = GameObject.Find("LaserPoint");
        UnifiedAttackRange(Instance.MapSizeX, AttackType.LaserGun);
    }
    public void IStateUpdate()
    {
        string direction = "";
        if (ready)
        {
            transform.position = new Vector3(
                Instance.player.transform.position.x + (Instance.player.transform.position.x - Instance.MousePos.x),
                Instance.player.transform.position.y + (Instance.player.transform.position.y - Instance.MousePos.y), 0);
            Instance.action.UpdateLookAtTarget(Instance.MousePos, transform, 0.001f, 30f);
        }
        if (!start && UpdateSelectionCheck())
        {
            direction = DirectionCheck(Instance.MyHit.positionInt);
            if (direction == "Right")
            {
                targetPos = new Vector3(0.5f, 10.5f, 0);
            }
            if (direction == "Left")
            {
                targetPos = new Vector3(10.5f, 0.5f, 0);
            }
            if (direction == "Up")
            {
                targetPos = new Vector3(0.5f, 0.5f, 0);
            }
            if (direction == "Down")
            {
                targetPos = new Vector3(10.5f, 10.5f, 0);
            }
            ready = false;
            start = true;
        }
        if (start)
        {
            bool move = !Instance.action.UpdateMove(transform, targetPos, 0.001f, 30f);
            bool look = !Instance.action.UpdateLookAtTarget(Instance.MyHit.positionInt, transform, 0.001f, 30f);
            laserPoint.transform.position = Instance.MyHit.positionInt;
            if (move && look)
            {
                direction = DirectionCheck(Instance.MyHit.positionInt);
                if (direction == "Right")
                {
                    targetPos = new Vector3(Instance.MapSizeX - 2, Instance.MyHit.positionInt.y, 0);
                }
                if (direction == "Left")
                {
                    targetPos = new Vector3(1, Instance.MyHit.positionInt.y, 0);
                }
                if (direction == "Up")
                {
                    targetPos = new Vector3(Instance.MyHit.positionInt.x, Instance.MapSizeY - 2, 0);
                }
                if (direction == "Down")
                {
                    targetPos = new Vector3(Instance.MyHit.positionInt.x, 1, 0);
                }
                Usage++;
                skillUse = true;
                start = false;
            }
        }
        if (skillUse)
        {
            Instance.action.UpdateMove(laserPoint.transform, targetPos, 0.001f, 10f);
            float line = Vector2.Distance(transform.position, laserPoint.transform.position);
            transform.GetChild(0).transform.localScale = new Vector3(line /2 + 0.5f, 0.15f, 0);
            Instance.action.UpdateLookAtTarget(laserPoint.transform.position, transform,0.001f, 10f);
            
            Vector3Int laserPointrPosInt = new Vector3Int(
                (int)Mathf.Round(laserPoint.transform.position.x),
                (int)Mathf.Round(laserPoint.transform.position.y),
                (int)Mathf.Round(transform.position.z)
            );
            if (laserPointPos != laserPointrPosInt)
            {
                laserPointPos = laserPointrPosInt;
                if (Instance.Map2D[laserPointrPosInt.x, laserPointrPosInt.y] == (int)MapObject.moster)
                {
                    Instance.poolManager.SelectPool(PoolManager.Prefabs.CrashBoxObject).Get().transform.position
                        = laserPointPos;
                }
            }
            if (Vector2.Distance(laserPoint.transform.position, targetPos) < 0.001f)
            {
                transform.GetChild(0).transform.localScale = new Vector3(0, 0, 0);
                Instance.playerState = State.Idle;
                ready = true;
                CheckUsage();
            }
        }
    }

    public bool Exit()
    {
        if (skillUse)
        {
            skillUse = false;
            return true;
        }
        return false;
    }
    

    public string DirectionCheck(Vector3Int mousePosInt)
    {
        Vector3 direction = mousePosInt - Instance.PlayerPositionInt;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                return "Right";
            }
            else
            {
                return "Left";
            }
        }
        else
        {
            if (direction.y > 0)
            {
                return "Up";
            }
            else
            {
                return "Down";
            }
        }
    }
}
