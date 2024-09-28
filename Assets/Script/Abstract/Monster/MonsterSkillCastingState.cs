using UnityEngine;

public abstract class MonsterSkillCastingState : MonoBehaviour, IState
{
    public Monster monster;
    public Player player;
    public MonsterMovement monsterMovement;
    public virtual void Awake()
    {
        monster = GetComponent<Monster>();
        player = FindObjectOfType<Player>();
        monsterMovement = GetComponent<MonsterMovement>();
    }
    public abstract void Entry();
    public abstract void IStateUpdate();
    public abstract void Exit();
}
