using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class MainButton : MonoBehaviour
{
    Sprite img;
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
        
        if (Instance.player.transform.Find("MainSkill").transform.childCount > 0)
        {
            if (img == Instance.player.transform.Find("MainSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite) return;

            GetComponent<Image>().sprite = Instance.player.transform.Find("MainSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            img = Instance.player.transform.Find("MainSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
    }
    public void OnClickMainButtion()
    {
        if (!Instance.playerTurn) return;

        if (Instance.player.transform.Find("MainSkill").transform.childCount > 0)
        {
            changeSkill = true;
            Instance.player.transform.Find("SideSkill").gameObject.SetActive(false);
            Instance.player.transform.Find("MainSkill").gameObject.SetActive(true);

            Instance.skillState = Instance.player.transform.Find("MainSkill").transform.GetChild(0).GetComponent<IState>();
        }   
    }
}
