using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;
        if (DrawDefaultInspector())
        {
            mapGenerator.DrawTexture2DNoiseMap();
        }
        if(GUILayout.Button("Generate Map"))
        {
            mapGenerator.DrawTexture2DNoiseMap();
        }


        if (GUI.changed)
        {
            UnityEditor.EditorUtility.SetDirty(target);
        }
    }

}
