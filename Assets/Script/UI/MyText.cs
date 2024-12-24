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
    private int CurrentStageCount = 0;
    private Vector3 stageTextPos = Vector3.zero;
    private void Awake()
    {
        stageTextPos = transform.position;

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
                    if (CurrentStageCount == GameManager.Instance.StageCount) return;
                    textMesh.text = "스테이지" + GameManager.Instance.StageCount.ToString();

                    if (!GameManager.Instance.action.UpdateMove(transform, new Vector3(-10000, transform.position.y, transform.position.z), 0.001f, 1500))
                    {
                        CurrentStageCount = GameManager.Instance.StageCount;
                        transform.position = new Vector3(stageTextPos.x, transform.position.y, transform.position.z);
                    }

                    break;
                }
            default:
                break;
        }
    }
}
