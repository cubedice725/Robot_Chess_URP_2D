using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class MainButton : MonoBehaviour
{
    Sprite img;
    private void Update()
    {
        if (Instance.player.transform.Find("MainSkill").transform.childCount > 0)
        {
            if (img == Instance.player.transform.Find("MainSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite) return;

            GetComponent<Image>().sprite = Instance.player.transform.Find("MainSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            img = Instance.player.transform.Find("MainSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
    }
}
