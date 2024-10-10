using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47Bullet : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(0.1f*Time.deltaTime,0,0);
    }
}
