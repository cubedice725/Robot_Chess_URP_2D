using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UiManager : MonoBehaviour
{
    public InputField affiliation;
    public InputField guestName;
    public GameObject guestRank;
    public string PlayerName;
    public string PlayserAffiliation;
    private string fileName = "Rank.csv";
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
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            affiliation = GameObject.Find("Affiliation").GetComponent<InputField>();
            guestName = GameObject.Find("GuestName").GetComponent<InputField>();
            GameObject.Find("GuestInformation").SetActive(false);
        }
    }
    private void Update()
    {
        if (start && SceneManager.GetActiveScene().name == "GameScene")
        {
            guestRank = GameObject.Find("GuestRank");
            start = false;
            if (guestRank != null) 
            {
                guestRank.SetActive(false);
            }
        }
        if (!end && SceneManager.GetActiveScene().name == "GameScene" && GameManager.Instance.player.Die == true)
        {
            time += Time.deltaTime;
            if (time > 3f)
            {
                if (guestRank != null)
                {
                    FindObjectOfType<Guests>().Initialize();
                    guestRank.SetActive(true);
                }
                end = true;
            }
        }
    }

    public void SaveToCSV()
    {
        string score;
        string path = Path.Combine(Application.persistentDataPath, fileName);
        List<string> guests = LoadFromCSV();
        int count = guests.Count; // 기본값으로 리스트의 끝을 설정

        for (int index = 0; index < guests.Count; index++)
        {
            string[] parts = guests[index].Split(',');
            if (parts.Length > 3)
            {
                score = parts[3].Trim();
                if (GameManager.Instance.GameScore > int.Parse(score))
                {
                    count = index;
                    break;
                }
            }
        }
        GameManager.Instance.poolManager.AllDistroyMyObject(PoolManager.Prefabs.Guest);
        // 새로운 데이터를 저장할 문자열 생성
        string newData = PlayerName + "," + PlayserAffiliation + "," + GameManager.Instance.GameTurnCount + "," + GameManager.Instance.GameScore;

        // count 위치에 새로운 데이터 삽입
        guests.Insert(count, newData);

        // 전체 리스트를 다시 파일에 쓰기
        using (StreamWriter writer = new StreamWriter(path, false)) // false로 설정하여 파일을 덮어쓰기
        {
            foreach (string guest in guests)
            {
                writer.WriteLine(guest);
            }
        }

        Debug.Log("Data saved to " + path);
        affiliation.text = "";
        guestName.text = "";
    }

    public List<string> LoadFromCSV()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        List<string> lines = new List<string>();
        if (File.Exists(path))
        {
            lines = File.ReadAllLines(path).ToList();
        }
        return lines;
    }
}
