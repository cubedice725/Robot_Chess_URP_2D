using Unity.VisualScripting.FullSerializer;
using static GameManager;
public class PlayerSkillCastingState : IState
{
    public void Entry()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.DamagedArea);
        Instance.skillState.Entry();
    }
    public void IStateUpdate()
    {
        Instance.skillState.IStateUpdate();
    }
    public bool Exit()
    {
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Selection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.UnSelection);
        Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.DamagedArea);
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



