using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBullet : MonoBehaviour
{
    public Vector3 targetPos;

    void FixedUpdate()
    {
        transform.Translate(20 * Time.fixedDeltaTime, 0, 0);
        
    }
    private void Update()
    {
        if (transform.position.x - targetPos.x < 0.1)
        {
            GetComponent<MyObject>().Destroy();
        }
    }
}
