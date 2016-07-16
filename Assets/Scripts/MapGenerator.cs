using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    public enum TextureMode
    {
        NoiseTexture2D,
        ColorNoiseTexture2D
    };
    public enum DrawMode
    {
        Texture2D,
        Mesh
    };
    public TextureWrapMode WrapMode = TextureWrapMode.Clamp;
    public FilterMode TextureFilterMode = FilterMode.Point;

    public DrawMode drawMode = DrawMode.Texture2D;
    public TextureMode textureMode = TextureMode.NoiseTexture2D;
    public int MeshLocalScaleMultiplier = 10;
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
    private GameObject OldTextureGameObject = null;

    public GameObject MeshToDrawOnGameObject = null;
    private GameObject OldMeshGameObject = null;

    public void DrawTexture2DNoiseMap()
    {
        var noiseMap = NoiseMap.GenerateNoiseMap(Size, Seed, Octaves, MaxOffSet, MinOffSet, OffSet, Persistence, Lacunarity, Scale);
        var texture2DNoiseMap = NoiseMap.ToDrawGetTexture2DNoiseMap(noiseMap);
        var texture2DColorNoiseMap = NoiseMap.ToDrawGetColorTexture2DNoiseMap(noiseMap, Assets);
        Texture2D texture2D = null;
        if (textureMode == TextureMode.NoiseTexture2D)
        {
            texture2D = texture2DNoiseMap;
        }
        else if (textureMode == TextureMode.ColorNoiseTexture2D)
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
        OldTextureGameObject = TextureToDrawOnGameObject;
    }

    public void DrawMeshNoiseMap()
    {
        var noiseMap = NoiseMap.GenerateNoiseMap(Size, Seed, Octaves, MaxOffSet, MinOffSet, OffSet, Persistence, Lacunarity, Scale);
        var texture2DNoiseMap = NoiseMap.ToDrawGetTexture2DNoiseMap(noiseMap);
        var texture2DColorNoiseMap = NoiseMap.ToDrawGetColorTexture2DNoiseMap(noiseMap, Assets);
        Texture2D texture2D = null;
        if (textureMode == TextureMode.NoiseTexture2D)
        {
            texture2D = texture2DNoiseMap;
        }
        else if (textureMode == TextureMode.ColorNoiseTexture2D)
        {
            texture2D = texture2DColorNoiseMap;
        }
        texture2D.filterMode = TextureFilterMode;
        texture2D.wrapMode = WrapMode;

        var meshAssets = NoiseMap.GenerateMeshAssestsFromNoiseMap(noiseMap);
        var mesh = NoiseMap.CreateMesh(meshAssets);

        if (MeshToDrawOnGameObject == null)
        {
            MeshToDrawOnGameObject = new GameObject();
            MeshToDrawOnGameObject.AddComponent<MeshFilter>();
            MeshToDrawOnGameObject.AddComponent<MeshRenderer>();
            MeshToDrawOnGameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

            MeshToDrawOnGameObject.name = "DrawNoiseMapMesh";
            MeshToDrawOnGameObject.transform.position = Vector3.zero;
            MeshToDrawOnGameObject.transform.localScale = Vector3.one * MeshLocalScaleMultiplier;

            var renderer = MeshToDrawOnGameObject.GetComponent<MeshRenderer>();

            renderer.material = new Material(Shader.Find("Standard"));
            renderer.sharedMaterial.mainTexture = texture2D;
            renderer.sharedMaterial.DisableKeyword("_NORMALMAP");
        }
        else
        {
            MeshToDrawOnGameObject.transform.localScale = Vector3.one * MeshLocalScaleMultiplier;

            MeshToDrawOnGameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

            var renderer = MeshToDrawOnGameObject.GetComponent<MeshRenderer>();

            renderer.sharedMaterial.mainTexture = texture2D;
            renderer.sharedMaterial.DisableKeyword("_NORMALMAP");
        }
        OldMeshGameObject = MeshToDrawOnGameObject;
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
        if (OldTextureGameObject != null)
        {
            DestroyImmediate(OldTextureGameObject);
            OldTextureGameObject = null;
            TextureToDrawOnGameObject = null;
        }
        if (OldMeshGameObject != null)
        {
            DestroyImmediate(OldMeshGameObject);
            OldMeshGameObject = null;
            MeshToDrawOnGameObject = null;
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

    public class MeshAssets
    {
        public Vector3 [] vertices;
        public int [] triangles;
        public Vector2 [] uvs;
        private int indexTriangles = 0;

        public void AddTriangles(int firstVecticesIndex,int secondVecticesIndex,int thirdVecticesIndex)
        {
            if(triangles.Length < firstVecticesIndex || triangles.Length < secondVecticesIndex || triangles.Length < thirdVecticesIndex)
            {
                Debug.LogError("Index Out Of Bounds For Adding Triangles Using Vertices Index");
            }
            else
            {
                triangles[indexTriangles] = firstVecticesIndex;
                triangles[indexTriangles + 1] = secondVecticesIndex;
                triangles[indexTriangles + 2] = thirdVecticesIndex;
                indexTriangles += 3;
            }
        }
    }
}