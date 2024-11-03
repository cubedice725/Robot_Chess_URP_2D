using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashBoxObject : MonoBehaviour
{
    public Collision2D collObject;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collObject = collision;
    }
}
