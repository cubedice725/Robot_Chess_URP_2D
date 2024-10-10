using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class UiManager : MonoBehaviour
{
    public void OnClickSkillTest()
    {
        if (!Instance.playerTurn) return;

        if (Instance.skillState != null)
        {
            Instance.playerState = State.Skill;
        }
    }
}
