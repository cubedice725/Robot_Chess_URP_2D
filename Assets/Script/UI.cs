using UnityEngine;
using static GameManager;

public class UI : MonoBehaviour
{
    public void OnClickSkillTest()
    {
        if (!Instance.playerTurn) return;

        if (Instance.skillState == null)
        {
            Instance.playerState = State.Skill;
        }
    }
}
