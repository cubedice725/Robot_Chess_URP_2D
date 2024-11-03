using UnityEngine;
using static GameManager;

public class UiManager : MonoBehaviour
{
    // �÷��̾�� ��ų�� ����ϱ� ���� ���𰡸� �����ߴ� MainSkill�� Ȯ����
    public void OnClickMainButtion()
    {
        if (!Instance.playerTurn) return;
        Instance.playerState = State.Idle;

        if (Instance.player.transform.Find("MainSkill").transform.childCount > 0)
        {
            Instance.player.transform.Find("SideSkill").gameObject.SetActive(false);
            Instance.player.transform.Find("MainSkill").gameObject.SetActive(true);

            Instance.skillState = Instance.player.transform.Find("MainSkill").transform.GetChild(0).GetComponent<IState>();
            TransitionTo(Instance.skillState);
            if (Instance.playerState == State.Idle)
            {
                Instance.playerState = State.Skill;
            }
        }
    }

    // �÷��̾�� ��ų�� ����ϱ� ���� ���𰡸� �����ߴ� SideSkill�� Ȯ����
    public void OnClickSideButtion()
    {
        if (!Instance.playerTurn) return;
        Instance.playerState = State.Idle;

        if (Instance.player.transform.Find("SideSkill").transform.childCount > 0)
        {
            Instance.player.transform.Find("MainSkill").gameObject.SetActive(false);
            Instance.player.transform.Find("SideSkill").gameObject.SetActive(true);

            Instance.skillState = Instance.player.transform.Find("SideSkill").transform.GetChild(0).GetComponent<IState>();
            TransitionTo(Instance.skillState);
            if(Instance.playerState == State.Idle)
            {
                Instance.playerState = State.Skill;
            }
        }
    }
    void TransitionTo(IState nextState)
    {
        if (nextState == Instance.skillState) return;
        Instance.skillState = nextState;
    }
}
