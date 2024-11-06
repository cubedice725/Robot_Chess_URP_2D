using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
public class TurnEndButton : MonoBehaviour
{
    Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    private void LateUpdate()
    {
        if (Instance.ButtonLock)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }
    public void OnClickTurnEndButton()
    {
        Instance.FromPlayerToMonster();
    }
}
