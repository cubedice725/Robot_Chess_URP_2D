using UnityEngine;

public abstract class MonsterIdleState : MonoBehaviour, IState
{
    public abstract void Enter();
    public abstract void IStateUpdate();
    public abstract void Exit();
}
