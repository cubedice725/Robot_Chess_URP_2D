using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnCount : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        text.text = "Turn Count " + GameManager.Instance.GameTurnCount.ToString();
    }
}
