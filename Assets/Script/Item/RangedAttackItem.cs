using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedAttackItem : MonoBehaviour
{
    bool start = false;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.transform.name);
        if (collision != null && collision.transform.name == "Player")
        {
            start = true;
        }
    }
    private void Update()
    {
        if (start) 
        {
            transform.localScale += new Vector3(1, 1, 0) * Time.deltaTime;
        }
    }
}
