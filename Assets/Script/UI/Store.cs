using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    public void StoreActiveTrue()
    {
        transform.gameObject.SetActive(true);
    }
    public void StoreActiveFalse()
    {
        transform.gameObject.SetActive(false);
    }
}
