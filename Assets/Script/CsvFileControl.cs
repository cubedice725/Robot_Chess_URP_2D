using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CsvFileControl : MonoBehaviour
{
    public Text displayText;
    public Button saveButton;
    public Button loadButton;
    public InputField affiliation;
    public InputField guestName;
    private string fileName = "Rank.csv";

    void SaveToCSV()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(affiliation.text + "," + guestName.text);
        }
        Debug.Log("Data saved to " + path);
        affiliation.text = "";
        guestName.text = "";
    }

    void LoadFromCSV()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            List<string> lines = File.ReadAllLines(path).ToList();
            displayText.text = string.Join("\n", lines);
        }
        else
        {
            Debug.LogWarning("File does not exist");
            displayText.text = "No data found";
        }
    }
}