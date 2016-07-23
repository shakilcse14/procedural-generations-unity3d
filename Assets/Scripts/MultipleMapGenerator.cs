using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleMapGenerator : MonoBehaviour
{
    public MapGenerator MapGen;
    public float MaxCameraViewDistance = 500.0f;
    public Transform Player;
    public Vector2 cameraViewerPosition;
    public int ChunkSize;
    public int NumberOfChunkToDraw;
    public Transform GenerateToObjectsChild;

    private int Co_OrdinateX;
    private int Co_OrdinateY;
    private Dictionary<Vector2, SingleMapHolder> ChunkHolderDictionary;
    private List<SingleMapHolder> lastVisibleChunk;


    void Start()
    {
        if (MapGen != null)
        {
            ChunkHolderDictionary = new Dictionary<Vector2, SingleMapHolder>();
            lastVisibleChunk = new List<SingleMapHolder>();
            ChunkSize = (MapGen.ChunkSize - 1) * MapGen.MeshLocalScaleMultiplier;
            NumberOfChunkToDraw = Mathf.RoundToInt(MaxCameraViewDistance / ChunkSize);
        }
    }

    void Update()
    {
        if (Player != null)
        {
            cameraViewerPosition = new Vector2(Player.position.x, Player.position.z);
            CheckInEveryLimitedMove();
            Player = null;
        }
    }

    void ClearUnWantedChunks()
    {
        //Debug.LogWarning("Clearing ... ..");
        for (int i = 0; i < lastVisibleChunk.Count; i++)
        {
            lastVisibleChunk[i].SetVisible(false);
        }
        lastVisibleChunk.Clear();
    }

    void CheckInEveryLimitedMove()
    {
        //ClearUnWantedChunks();
        Co_OrdinateX = Mathf.RoundToInt(cameraViewerPosition.x / ChunkSize);
        Co_OrdinateY = Mathf.RoundToInt(cameraViewerPosition.y / ChunkSize);
        for (int y = -NumberOfChunkToDraw; y <= NumberOfChunkToDraw; y++)
        {
            for (int x = -NumberOfChunkToDraw; x <= NumberOfChunkToDraw; x++)
            {
                Vector2 pos = new Vector2(Co_OrdinateX + x, Co_OrdinateY + y);
                if (ChunkHolderDictionary.ContainsKey(pos))
                {
                    ChunkHolderDictionary[pos].UpdateSingleMapEveryTime(cameraViewerPosition);
                    if(ChunkHolderDictionary[pos].IsVisible())
                    {
                        lastVisibleChunk.Add(ChunkHolderDictionary[pos]);
                    }
                }
                else
                {
                    if (GenerateToObjectsChild != null)
                    {
                        ClearUnWantedChunks();
                        ChunkHolderDictionary.Add(pos, new SingleMapHolder(pos, ChunkSize,
                            GenerateToObjectsChild, MaxCameraViewDistance, MapGen));
                    }
                }
            }
        }
    }

    public class SingleMapHolder
    {
        public GameObject MapChunkObject;
        public Vector2 Position;
        Bounds Bound;
        float MaxCameraViewDistance;
        MapGenerator _MapGenerator;

        public SingleMapHolder(Vector2 pos, int SingleChunkSize, Transform Parent, float MaxCamViewDist, MapGenerator mapGenerator)
        {
            this._MapGenerator = mapGenerator;
            MaxCameraViewDistance = MaxCamViewDist;
            Position = new Vector2(pos.x * SingleChunkSize, pos.y * SingleChunkSize);
            Bound = new Bounds(new Vector3(Position.x, Position.y, 0), Vector3.one
                * SingleChunkSize * mapGenerator.MeshLocalScaleMultiplier);
            
            
            MapChunkObject = new GameObject();
            MapChunkObject.name = "DrawMesh";
            MapChunkObject = GenerateMeshComponents(MapChunkObject);
            MapChunkObject.transform.position = new Vector3(Position.x, 0, Position.y);
            MapChunkObject.transform.localScale = Vector3.one * mapGenerator.MeshLocalScaleMultiplier;

            
            MapChunkObject.transform.parent = Parent;
            //SetVisible(false);
            _MapGenerator.RequestData(Position, OnDataReceived);
        }

        void OnDataReceived(MapGenerator.Data data)
        {
            Debug.LogWarning("Data Received");
            MapChunkObject.GetComponent<MeshFilter>().mesh = NoiseMap.CreateMesh(data.meshAssets);

            var renderer = MapChunkObject.GetComponent<MeshRenderer>();
            MapChunkObject.name = data.name;

            renderer.material = new Material(Shader.Find("Standard"));
            renderer.material.mainTexture = NoiseMap.ToDrawGetColorTexture2DNoiseMap(data.noiseMap, _MapGenerator.Assets);
            renderer.sharedMaterial.DisableKeyword("_NORMALMAP");
        }

        GameObject GenerateMeshComponents(GameObject meshGameObject)
        {
            meshGameObject.AddComponent<MeshFilter>();
            meshGameObject.AddComponent<MeshRenderer>();
            
            //meshGameObject.transform.localScale = Vector3.one * MeshLocalScaleMultiplier;

            return meshGameObject;
        }

        public void UpdateSingleMapEveryTime(Vector2 cameraViewPos)
        {
            float viewLastEdgeDistanceFromViewer = Mathf.Sqrt(Bound.SqrDistance(cameraViewPos));
            //Debug.LogWarning(viewLastEdgeDistanceFromViewer);
            bool isVisible = viewLastEdgeDistanceFromViewer <= MaxCameraViewDistance;
            SetVisible(isVisible);
        }

        public void SetVisible(bool Enable)
        {
            MapChunkObject.SetActive(Enable);
        }
        public bool IsVisible()
        {
            return MapChunkObject.activeSelf;
        }
    }
}