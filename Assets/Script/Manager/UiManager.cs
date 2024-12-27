using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static MyButton;
using UnityEditor;
public class UiManager : MonoBehaviour
{
    public TMP_InputField affiliation;
    public TMP_InputField guestName;
    public GameObject GameOver;
    public string PlayerName;
    public string PlayserAffiliation;
    private bool start = true;
    private bool end = false;

    private float time;

    private static UiManager _instance;
    public static UiManager Instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(UiManager)) as UiManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }
    public void Reset()
    {
        start = true;
        end = false;
    }
    private void Update()
    {
        if (start && SceneManager.GetActiveScene().name == "GameScene")
        {
            GameOver = GameObject.Find("GameOver");
            if (GameOver != null)
            {
                start = false;
                GameOver.SetActive(false);
            }
        }
        if (!end && SceneManager.GetActiveScene().name == "GameScene" && GameManager.Instance.player.Die == true)
        {
            time += Time.deltaTime;
            if (time > 2f)
            {
                print(2);
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.Reset();
                    SceneManager.LoadScene("GameScene");
                    UiManager.Instance.Reset();
                }
                else
                {
                    SceneManager.LoadScene("GameScene");
                    UiManager.Instance.Reset();
                }
                time = 0;
                end = true;
            }
            else if (time > 3f)
            {
                print(1);
                if (GameOver != null)
                {
                    GameOver.SetActive(true);
                }
            }
            
        }
    }
}
