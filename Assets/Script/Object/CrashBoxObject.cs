using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashBoxObject : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null)
        {
            collision.transform.GetComponent<Monster>().HP -= 1;
            GetComponent<MyObject>().Destroy();
        }
    }
}
