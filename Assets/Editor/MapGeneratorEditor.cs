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
            if (mapGenerator.drawMode == MapGenerator.DrawMode.OnlyTexture2D)
            {
                mapGenerator.DrawTexture2DNoiseMap();
            }
        }
        if(GUILayout.Button("Generate Map"))
        {
            if (mapGenerator.drawMode == MapGenerator.DrawMode.OnlyTexture2D)
            {
                mapGenerator.DrawTexture2DNoiseMap();
            }
        }


        if (GUI.changed)
        {
            UnityEditor.EditorUtility.SetDirty(target);
        }
    }
}
