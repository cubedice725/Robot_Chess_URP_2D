using UnityEngine;

public abstract class MonsterMovingState : MonoBehaviour, IState
{
    public Monster monster;
    public MonsterMovement monsterMovement;
    public virtual void Awake()
    {
        monster = GetComponent<Monster>();
        monsterMovement = GetComponent<MonsterMovement>();
    }
    public abstract void Entry();
    public abstract void IStateUpdate();
    public abstract void Exit();
}
