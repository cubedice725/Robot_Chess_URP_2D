using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObject : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)GameManager.MapObject.wall;
    }
}
