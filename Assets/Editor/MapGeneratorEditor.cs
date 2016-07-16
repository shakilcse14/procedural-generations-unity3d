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
            if (mapGenerator.drawMode == MapGenerator.DrawMode.Texture2D)
            {
                mapGenerator.DrawTexture2DNoiseMap();
            }
            else if (mapGenerator.drawMode == MapGenerator.DrawMode.Mesh)
            {
                mapGenerator.DrawMeshNoiseMap();
            }
        }
        if(GUILayout.Button("Generate Map"))
        {
            if (mapGenerator.drawMode == MapGenerator.DrawMode.Texture2D)
            {
                mapGenerator.DrawTexture2DNoiseMap();
            }
            else if (mapGenerator.drawMode == MapGenerator.DrawMode.Mesh)
            {
                mapGenerator.DrawMeshNoiseMap();
            }
        }


        if (GUI.changed)
        {
            UnityEditor.EditorUtility.SetDirty(target);
        }
    }

}
