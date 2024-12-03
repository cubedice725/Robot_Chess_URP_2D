using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class EscButton : MonoBehaviour
{
    void Update()
    {
        if (!Instance.ButtonLock)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                transform.gameObject.SetActive(false);
                if (GameManager.Instance.player.Die)
                {
                    GameManager.Instance.Reset();
                    UiManager.Instance.Reset();
                    SceneManager.LoadScene("MainScene");
                }
            }
        }
    }
}
