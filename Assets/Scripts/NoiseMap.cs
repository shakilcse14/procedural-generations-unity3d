using UnityEngine;
using System.Collections;

public static class NoiseMap
{
    public static float[,] GenerateNoiseMap(MapGenerator.NormalizeNoiseMapMode normalMapMode, int mapSize, int seedValue, 
        int octaves, int maxOffsetOctaves, int minOffsetOctaves, Vector2 offset, float persistence,
        float lacunarity, float scale = 0.1f)
    {
        if(octaves < 0)
        {
            octaves = 0;
        }
        System.Random rnd = new System.Random(seedValue);
        Vector2[] octaveOffset = new Vector2[octaves];
        float HalfSize = mapSize / 2.0f;
        float maxGlobalHeight = 0.0f;
        float amplitude = 1;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rnd.Next(minOffsetOctaves, maxOffsetOctaves) + offset.x;
            float offsetY = rnd.Next(minOffsetOctaves, maxOffsetOctaves) - offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
            maxGlobalHeight += amplitude;
            amplitude *= persistence;
        }

        float[,] noiseMap = new float[mapSize, mapSize];
        if(scale <= 0.0f)
        {
            scale = 0.1f;
        }
        float minNoiseMapHeight = float.MaxValue;
        float maxNoiseMapHeight = float.MinValue;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseMapHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float samepleX = (x - HalfSize + octaveOffset[i].x) / scale * frequency;
                    float samepleY = (y - HalfSize + octaveOffset[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(samepleX, samepleY) * 2 -1;
                    noiseMapHeight += perlinValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                if (noiseMapHeight > maxNoiseMapHeight)
                {
                    maxNoiseMapHeight = noiseMapHeight;
                }
                else if (noiseMapHeight < minNoiseMapHeight)
                {
                    minNoiseMapHeight = noiseMapHeight;
                }
                noiseMap[x, y] = noiseMapHeight;
            }
        }
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                if (normalMapMode == MapGenerator.NormalizeNoiseMapMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseMapHeight, maxNoiseMapHeight, noiseMap[x, y]);
                }
                else if(normalMapMode == MapGenerator.NormalizeNoiseMapMode.Global)
                {
                    var normalizedValue = noiseMap[x, y] + 1 / ( 2f * maxGlobalHeight / 1.75f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedValue, 0, int.MaxValue); ;
                }
            }
        }
        return noiseMap;
    }

    public static Texture2D ToDrawGetTexture2DNoiseMap(float[,] noiseMap)
    {
        int noiseMapWidth = noiseMap.GetLength(0);
        int noiseMapHeight = noiseMap.GetLength(1);

        Texture2D texture2D = new Texture2D(noiseMapWidth, noiseMapHeight);
        Color[] color = new Color[noiseMapWidth * noiseMapHeight];

        for (int y = 0; y < noiseMapHeight; y++)
        {
            for (int x = 0; x < noiseMapWidth; x++)
            {
                color[y * noiseMapHeight + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture2D.SetPixels(color);
        texture2D.Apply();
        return texture2D;
    }

    public static Texture2D ToDrawGetColorTexture2DNoiseMap(float [,] noiseMap, MapGenerator.MapAssets [] assets)
    {
        int noiseMapWidth = noiseMap.GetLength(0);
        int noiseMapHeight = noiseMap.GetLength(1);

        Texture2D texture2D = new Texture2D(noiseMapWidth, noiseMapHeight);
        Color[] color = new Color[noiseMapWidth * noiseMapHeight];

        for (int y = 0; y < noiseMapHeight; y++)
        {
            for (int x = 0; x < noiseMapWidth; x++)
            {
                for (int i = 0; i < assets.Length; i++)
                {
                    if (assets[i].HeightInMap <= noiseMap[x, y])
                    {
                        color[y * noiseMapHeight + x] = assets[i].color;

                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        texture2D.SetPixels(color);
        texture2D.Apply();
        return texture2D;
    }

    public static MapGenerator.MeshAssets GenerateMeshAssestsFromNoiseMap(float[,] noiseMap,
        float noiseMapMultiplier, AnimationCurve curve, int lOD, int chunkSize)
    {
        AnimationCurve animationCurve = new AnimationCurve(curve.keys);
        int noiseMapWidth = noiseMap.GetLength(0);
        int noiseMapHeight = noiseMap.GetLength(1);

        MapGenerator.MeshAssets meshAssets = new MapGenerator.MeshAssets();
        int increment = (lOD == 0 ? 1 : lOD * 2);
        int perLineVectices = (chunkSize - 1) / increment + 1;
        meshAssets.vertices = new Vector3[perLineVectices * perLineVectices];
        meshAssets.triangles = new int[(perLineVectices - 1) * (perLineVectices - 1) * 6];
        meshAssets.uvs = new Vector2[perLineVectices * perLineVectices];
        float topLeftX = (noiseMapWidth - 1) / -2f;
        float topLeftZ = (noiseMapWidth - 1) / 2f;
        int indexVertices = 0;

        for (int y = 0; y < noiseMapHeight; y += increment)
        {
            for (int x = 0; x < noiseMapWidth; x += increment)
            {
                meshAssets.vertices[indexVertices] = new Vector3(topLeftX + x,
                    animationCurve.Evaluate(noiseMap[x, y]) * noiseMapMultiplier, topLeftZ - y);
                meshAssets.uvs[indexVertices] = new Vector2(x / (float)noiseMapWidth, y / (float)noiseMapHeight);
                if (x < noiseMapWidth - 1 && y < noiseMapHeight - 1)
                {
                    meshAssets.AddTriangles(indexVertices, indexVertices + perLineVectices + 1, indexVertices + perLineVectices);
                    meshAssets.AddTriangles(indexVertices + perLineVectices + 1, indexVertices, indexVertices + 1);
                }
                indexVertices++;
            }
        }
        return meshAssets;
    }

    public static Mesh CreateMesh(MapGenerator.MeshAssets meshAssests)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = meshAssests.vertices;
        mesh.triangles = meshAssests.triangles;
        mesh.uv = meshAssests.uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
