using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Array : MonoBehaviour
{
    public int[,] array;
    public int sizeX;
    public int sizeY;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            array = GameManager.Instance.Map2D;
            sizeX = GameManager.Instance.MapSizeX;
            sizeY = GameManager.Instance.MapSizeY;
        }
    }
}

[CustomEditor(typeof(Array))]
public class ArrayEditor : Editor
{
    Array arrayComponent;

    void OnEnable()
    {
        arrayComponent = (Array)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (arrayComponent.array == null) return;

        EditorGUILayout.LabelField("2���� �迭");

        for (int x = 0; x < arrayComponent.sizeX; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < arrayComponent.sizeY; y++)
            {
                arrayComponent.array[x, y] = EditorGUILayout.IntField(arrayComponent.array[x, y]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(arrayComponent);
        }
    }
}