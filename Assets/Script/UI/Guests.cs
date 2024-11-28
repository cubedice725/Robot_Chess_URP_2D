using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Guests : MonoBehaviour
{
    List<string> guests = new List<string>();
    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        guests = UiManager.Instance.LoadFromCSV();
        for (int index = 0; index < guests.Count; index++)
        {
            MyObject myObject;
            myObject = GameManager.Instance.poolManager.SelectPool(PoolManager.Prefabs.Guest).Get();
            myObject.transform.SetParent(transform);
            myObject.transform.localScale = Vector3.one;
            string[] parts = guests[index].Split(',');
            myObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (index + 1) + "µÓ " + parts[1] + parts[0] + parts[2] + "≈œ" + parts[3] + "¡°";
        }
    }
}
