using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackObject : MonoBehaviour
{
    bool start = false;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.transform.name);
        if (collision != null && collision.transform.name == "Player")
        {
            start = true;
        }
        if(collision != null && collision.transform.GetComponent<Monster>() != null && start)
        {
            collision.transform.GetComponent<Monster>().HP -= 1;
        }
    }
    private void Update()
    {
        if (start)
        {
            float y = Time.deltaTime * 10;
            transform.localScale += new Vector3(1, 1, 0) * y;

            if (transform.localScale.x >= 3)
            {
                transform.localScale = new Vector3(0.5f, 0.5f, 0);
                start = false;
                GetComponent<MyObject>().Destroy();
            }
        }
    }
}
