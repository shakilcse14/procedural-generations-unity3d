using UnityEngine;
using System.Collections;
using UnityEditor;

public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        base.OnInspectorGUI();
    }
}
