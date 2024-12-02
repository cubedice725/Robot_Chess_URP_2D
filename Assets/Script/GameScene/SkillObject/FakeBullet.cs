using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBullet : MonoBehaviour
{
    public Vector3 targetPos;

    private Vector3 previousPosition;
    bool start = true;
    void FixedUpdate()
    {
        transform.Translate(15 * Time.fixedDeltaTime, 0, 0);
    }
    void Update()
    {
        if (start)
        {
            previousPosition = transform.position;
            start = false;
        }
        Vector3 currentPosition = transform.position;

        float previousDistance = Vector3.Distance(previousPosition, targetPos);
        float currentDistance = Vector3.Distance(currentPosition, targetPos);

        if (previousDistance < currentDistance)
        {
            start = true;
            GetComponent<MyObject>().Destroy();
        }
        previousPosition = currentPosition;
    }
}