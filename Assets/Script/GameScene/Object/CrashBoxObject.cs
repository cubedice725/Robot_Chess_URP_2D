using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashBoxObject : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null && collision.transform.GetComponent<Monster>() != null)
        {
            collision.transform.GetComponent<Monster>().Hp -= 1;
            GetComponent<MyObject>().Destroy();
        }
        if (collision != null && collision.transform.GetComponent<Player>() != null)
        {
            collision.transform.GetComponent<Player>().Hp -= 1;
            GetComponent<MyObject>().Destroy();
        }
    }
}
