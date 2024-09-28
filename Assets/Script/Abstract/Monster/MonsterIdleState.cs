using UnityEngine;

public abstract class MonsterIdleState : MonoBehaviour, IState
{
    public abstract void Entry();
    public abstract void IStateUpdate();
    public abstract void Exit();
}
