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
        TurnCount,
        MainCount,
        SideCount,
        Stage
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
            case MyTextOption.MainCount:
                {
                    if (GameManager.Instance.player.transform.Find("MainSkill").transform.childCount > 0)
                    {
                        Skill skill = GameManager.Instance.player.transform.Find("MainSkill").transform.GetChild(0).transform.GetComponent<Skill>();
                        textMesh.text = (skill.UsageLimit - skill.Usage).ToString();
                    }
                        
                    break;
                }
            case MyTextOption.SideCount:
                {
                    if (GameManager.Instance.player.transform.Find("SideSkill").transform.childCount > 0)
                    {
                        Skill skill = GameManager.Instance.player.transform.Find("SideSkill").transform.GetChild(0).transform.GetComponent<Skill>();
                        textMesh.text = (skill.UsageLimit - skill.Usage).ToString();
                    }
                    break;
                }
            case MyTextOption.Stage:
                {
                    if(GameManager.Instance.StageCount == 1)
                    {
                        StartCoroutine(Show());
                    }
                    break;
                }
            default:
                break;
        }
    }
    IEnumerator Show()
    {
        transform.position = new Vector3(0, 0, 0);
        textMesh.text = "스테이지" + GameManager.Instance.StageCount.ToString();
        yield return new WaitForSeconds(0.5f);
        transform.position = new Vector3(-10000, -10000, 0);
    }

}
