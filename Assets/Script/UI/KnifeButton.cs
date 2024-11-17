using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class KnifeButton : MonoBehaviour
{
    Sprite img;
    Button button;
    bool changeSkill = false;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
    private void Update()
    {
        if (Instance.ButtonLock)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
        if (changeSkill && Instance.playerState == State.Idle)
        {
            Instance.playerState = State.Skill;
            changeSkill = false;
        }
        if (changeSkill)
        {
            Instance.playerState = State.Idle;
        }
    }
    public void OnClickKnifeButton()
    {
        if (!Instance.playerTurn) return;

        changeSkill = true;
        Instance.player.transform.Find("SideSkill").gameObject.SetActive(false);
        Instance.player.transform.Find("MainSkill").gameObject.SetActive(false);
        Instance.player.transform.Find("KnifeSkill").gameObject.SetActive(true);

        Instance.skillState = Instance.player.transform.Find("KnifeSkill").transform.GetChild(0).GetComponent<IState>();
    }
}
