using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyText : MonoBehaviour
{
    public enum MyTextOption
    {
        None,
        AttackTurn,
        MoveTurn,
        Score,
        TurnCount
    }
    public TextMeshProUGUI textMesh;
    public MyTextOption myTextOption;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        switch (myTextOption)
        {
            case MyTextOption.AttackTurn:
                {
                    textMesh.text = GameManager.Instance.player.AttackCount.ToString() + "/1";
                    break;
                }
            case MyTextOption.MoveTurn:
                {
                    textMesh.text = GameManager.Instance.player.MoveCount.ToString() + "/1";
                    break;
                }
            case MyTextOption.Score:
                {
                    textMesh.text = "Score " + GameManager.Instance.GameScore.ToString();
                    break;
                }
            case MyTextOption.TurnCount:
                {
                    textMesh.text = "Turn Count " + GameManager.Instance.GameTurnCount.ToString();
                    break;
                }
            default:
                break;
        }
    }
}
