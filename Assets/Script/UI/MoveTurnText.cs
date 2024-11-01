using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveTurnText : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        text.text = GameManager.Instance.player.MoveCount.ToString() + "/1";
    }
}
