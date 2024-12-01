using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static MyButton;
public class UiManager : MonoBehaviour
{
    public TMP_InputField affiliation;
    public TMP_InputField guestName;
    public GameObject guestRank;
    public Guests guests;
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
            // �ν��Ͻ��� ���� ��쿡 �����Ϸ� �ϸ� �ν��Ͻ��� �Ҵ����ش�.
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
            // �ν��Ͻ��� �����ϴ� ��� ���λ���� �ν��Ͻ��� �����Ѵ�.
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
            guestRank = GameObject.Find("GuestRank");
            guests = FindObjectOfType<Guests>();
            if (guestRank != null)
            {
                start = false;

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
                    guests.Initialize();
                    guestRank.SetActive(true);
                    time = 0;
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
        int count = guests.Count; // �⺻������ ����Ʈ�� ���� ����

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
        // ���ο� �����͸� ������ ���ڿ� ����
        string newData = PlayerName + "," + PlayserAffiliation + "," + GameManager.Instance.GameTurnCount + "," + GameManager.Instance.GameScore;

        // count ��ġ�� ���ο� ������ ����
        guests.Insert(count, newData);

        // ��ü ����Ʈ�� �ٽ� ���Ͽ� ����
        using (StreamWriter writer = new StreamWriter(path, false)) // false�� �����Ͽ� ������ �����
        {
            foreach (string guest in guests)
            {
                writer.WriteLine(guest);
            }
        }

        Debug.Log("Data saved to " + path);
        if(affiliation != null)
        {
            affiliation.text = "";
            guestName.text = "";
        }
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
