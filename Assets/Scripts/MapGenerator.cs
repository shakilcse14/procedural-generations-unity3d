using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseTexture2D,
        ColorNoiseTexture2D,
        Mesh
    };
    public TextureWrapMode WrapMode = TextureWrapMode.Clamp;
    public FilterMode TextureFilterMode = FilterMode.Point;

    public DrawMode drawMode = DrawMode.NoiseTexture2D;
    public int Size = 50;
    public float Scale = 0.1f;
    public int Seed = 0;
    public int Octaves = 0;
    public int MaxOffSet = 1000;
    public int MinOffSet = -1000;
    public Vector2 OffSet = Vector2.zero;
    [Range(0.0f, 1.0f)]
    public float Persistence = 0;
    public float Lacunarity = 0;
    public MapAssets[] Assets;


    public GameObject TextureToDrawOnGameObject = null;
    private GameObject OldGameObject = null;

    public void DrawTexture2DNoiseMap()
    {
        var noiseMap = NoiseMap.GenerateNoiseMap(Size, Seed, Octaves, MaxOffSet, MinOffSet, OffSet, Persistence, Lacunarity, Scale);
        var texture2DNoiseMap = NoiseMap.ToDrawGetTexture2DNoiseMap(noiseMap);
        var texture2DColorNoiseMap = NoiseMap.ToDrawGetColorTexture2DNoiseMap(noiseMap, Assets);
        Texture2D texture2D = null;
        if(drawMode == DrawMode.NoiseTexture2D)
        {
            texture2D = texture2DNoiseMap;
        }
        else if (drawMode == DrawMode.ColorNoiseTexture2D)
        {
            texture2D = texture2DColorNoiseMap;
        }

        if (TextureToDrawOnGameObject == null)
        {
            TextureToDrawOnGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            TextureToDrawOnGameObject.name = "DrawNoiseMapTexture2D";
            DestroyImmediate(TextureToDrawOnGameObject.GetComponent<Collider>());
            TextureToDrawOnGameObject.transform.position = Vector3.zero;
            TextureToDrawOnGameObject.transform.localScale = new Vector3(Size, 1, Size);
            Renderer renderer = TextureToDrawOnGameObject.transform.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Standard"));
            texture2D.filterMode = TextureFilterMode;
            texture2D.wrapMode = WrapMode;
            renderer.sharedMaterial.mainTexture = texture2D;
            renderer.sharedMaterial.DisableKeyword("_NORMALMAP");
        }
        else
        {
            TextureToDrawOnGameObject.transform.localScale = new Vector3(Size, 1, Size);
            Renderer renderer = TextureToDrawOnGameObject.transform.GetComponent<Renderer>();
            texture2D.filterMode = TextureFilterMode;
            texture2D.wrapMode = WrapMode;
            renderer.sharedMaterial.mainTexture = texture2D;
            renderer.sharedMaterial.DisableKeyword("_NORMALMAP");
        }
        OldGameObject = TextureToDrawOnGameObject;
    }

    public void OnValidate()
    {
        if (Size < 1)
        {
            Size = 1;
        }
        if (Octaves < 0)
        {
            Octaves = 1;
        }
    }

    public void Reset()
    {
        if (OldGameObject != null)
        {
            DestroyImmediate(OldGameObject);
            OldGameObject = null;
            TextureToDrawOnGameObject = null;
        }
    }
    [System.Serializable]
    public class MapAssets
    {
        public string Type;
        public float HeightInMap;
        public enum DrawType
        {
            Color,
            Texture2D
        };
        public DrawType Draw;
        public Color color;
    }
}