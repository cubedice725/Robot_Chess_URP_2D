using UnityEngine;
#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using UnityEditor;
#endif

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
#if UNITY_EDITOR
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

        EditorGUILayout.LabelField("2차원 배열");

        for (int y = arrayComponent.sizeY - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < arrayComponent.sizeX; x++)
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
#endif