using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {
    public enum DrawMode
    {
        OnlyTexture2D,
        Mesh
    };
    public DrawMode drawMode = DrawMode.OnlyTexture2D;
    public int Size = 50;
    public float Scale = 0.1f;

    public GameObject TextureToDrawOnGameObject = null;

    public void DrawTexture2DNoiseMap()
    {
        var noiseMap = NoiseMap.GenerateNoiseMap(Size, Scale);
        var texture2DNoiseMap = NoiseMap.ToDrawGetTexture2DNoiseMap(noiseMap);

        if (TextureToDrawOnGameObject == null)
        {
            TextureToDrawOnGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            TextureToDrawOnGameObject.name = "DrawNoiseMapTexture2D";
            DestroyImmediate(TextureToDrawOnGameObject.GetComponent<Collider2D>());
            TextureToDrawOnGameObject.transform.position = Vector3.zero;
            TextureToDrawOnGameObject.transform.localScale = new Vector3(Size, 1, Size);
            Renderer renderer = TextureToDrawOnGameObject.transform.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Standard"));
            renderer.sharedMaterial.mainTexture = texture2DNoiseMap;
            renderer.sharedMaterial.DisableKeyword("_NORMALMAP");
        }
        else
        {
            TextureToDrawOnGameObject.transform.localScale = new Vector3(Size, 1, Size);
            Renderer renderer = TextureToDrawOnGameObject.transform.GetComponent<Renderer>();
            renderer.sharedMaterial.mainTexture = texture2DNoiseMap;
            renderer.sharedMaterial.DisableKeyword("_NORMALMAP");
        }

    }
}
