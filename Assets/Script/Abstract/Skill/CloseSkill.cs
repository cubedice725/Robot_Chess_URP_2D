using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
public class CloseSkill : Skill
{
    public void AttackRange(int size)
    {
        for (int i = 1; i <= size; i++)
        {
            try
            {
                if (Instance.Map2D[Instance.PlayerPositionInt.x - i, Instance.PlayerPositionInt.y] == (int)MapObject.moster)
                {
                    Instance.SetSelection(new Vector2(Instance.PlayerPositionInt.x - i, Instance.PlayerPositionInt.y));
                }
                if (Instance.Map2D[Instance.PlayerPositionInt.x + i, Instance.PlayerPositionInt.y] == (int)MapObject.moster)
                {
                    Instance.SetSelection(new Vector2(Instance.PlayerPositionInt.x + i, Instance.PlayerPositionInt.y));
                }
                if (Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y - i] == (int)MapObject.moster)
                {
                    Instance.SetSelection(new Vector2(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y - i));
                }
                if (Instance.Map2D[Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y + i] == (int)MapObject.moster)
                {
                    Instance.SetSelection(new Vector2(Instance.PlayerPositionInt.x, Instance.PlayerPositionInt.y + i));
                }
            }
            catch { }
        }
    }

    public void UpAttack()
    {

    }
    public void DownAttack()
    {

    }
    public void LeftAttack()
    {

    }
    public void RightAttack() 
    { 

    }
}
