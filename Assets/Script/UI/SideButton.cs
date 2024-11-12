using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SideButton : MonoBehaviour
{
    Sprite img;
    public Sprite veto;

    bool changeSkill = false;
    Button button;
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

        if (Instance.player.transform.Find("SideSkill").transform.childCount > 0)
        {
            if (img == Instance.player.transform.Find("SideSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite) return;

            GetComponent<Image>().sprite = Instance.player.transform.Find("SideSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            img = Instance.player.transform.Find("SideSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            GetComponent<Image>().sprite = veto;
        }
    }
    public void OnClickSideButtion()
    {
        if (!Instance.playerTurn) return;

        if (Instance.player.transform.Find("SideSkill").transform.childCount > 0)
        {
            changeSkill = true;
            Instance.player.transform.Find("MainSkill").gameObject.SetActive(false);
            Instance.player.transform.Find("SideSkill").gameObject.SetActive(true);

            Instance.skillState = Instance.player.transform.Find("SideSkill").transform.GetChild(0).GetComponent<IState>();
        }
    }
}
