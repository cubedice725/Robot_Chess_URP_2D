using Unity.VisualScripting.FullSerializer;
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
    public bool Exit()
    {
        if (Instance.skillState.Exit())
        {
            Instance.player.AttackCount++;
            Instance.ButtonLock = false;

            return true;
        }
        Instance.ButtonLock = false;
        return false;
    }
}



