using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Guests : MonoBehaviour
{
    List<string> guests = new List<string>();
    public bool start = false;
    private void Start()
    {
        start = true;
    }
    private void Update()
    {
        if (start && GameManager.Instance.poolManager != null)
        {
            Initialize();
            start = false;
        }
    }
    public void Initialize()
    {
        while (GameManager.Instance.poolManager.MyObjectLists[(int)PoolManager.Prefabs.Guest].Count < guests.Count)
        {
            MyObject myObject;
            myObject = GameManager.Instance.poolManager.SelectPool(PoolManager.Prefabs.Guest).Get();
            myObject.transform.SetParent(transform);
            myObject.transform.localScale = Vector3.one;
        }

        for (int index = 0; index < guests.Count; index++)
        {
            string[] parts = guests[index].Split(',');
            GameManager.Instance.poolManager.MyObjectLists[(int)PoolManager.Prefabs.Guest][index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            GameManager.Instance.poolManager.MyObjectLists[(int)PoolManager.Prefabs.Guest][index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (index + 1) + "µÓ " + parts[1] + " " + parts[0] + " " + parts[2] + "≈œ " + parts[3] + "¡°";
        }
    }
}
