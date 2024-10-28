using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SideButton : MonoBehaviour
{
    Sprite img;
    private void Update()
    {
        if (Instance.player.transform.Find("SideSkill").transform.childCount > 0)
        {
            if (img == Instance.player.transform.Find("SideSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite) return;

            GetComponent<Image>().sprite = Instance.player.transform.Find("SideSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            img = Instance.player.transform.Find("SideSkill").transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
    }
}
