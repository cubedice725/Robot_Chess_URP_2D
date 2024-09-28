using UnityEngine;
using static GameManager;
public class PlayerSkillCastingState : IState
{
    public void Entry()
    {
        Instance.skillState.Entry();
    }
    public void IStateUpdate()
    {
        Instance.skillState.IStateUpdate();
    }
    public void Exit()
    {
        Instance.skillState.Exit();
        Instance.skillState = null;
    }
}
