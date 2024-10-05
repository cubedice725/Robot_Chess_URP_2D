using UnityEngine;

public class RobotSkillCastingState : MonoBehaviour, IState
{
    MonsterMovement monsterMovement;
    public void Awake()
    {
        monsterMovement = GetComponent<MonsterMovement>();
    }
    public void Entry()
    {
        print("АјАн");
        monsterMovement.IdleState();
    }
    public void IStateUpdate()
    {
    }
    public bool Exit()
    {
        return true;
    }
}
