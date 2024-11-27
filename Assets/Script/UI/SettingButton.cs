using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingButton : MonoBehaviour
{
    public enum Option
    {
        Rank,
        Again,
        Close
    }
    public Option option;
    public void OnClickSetting()
    {
        switch (option)
        {
            case Option.Rank:
                {

                    break;
                }
            case Option.Again:
                {
                    GameManager.Instance.Reset();
                    SceneManager.LoadScene("SampleScene");
                    break;
                }
            case Option.Close:
                {
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                    break;
                }
            default:
                break;
        }
    }
}
