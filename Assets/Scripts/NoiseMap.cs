using UnityEngine;
using System.Collections;

public static class NoiseMap
{
    public static float[,] GenerateNoiseMap(int mapSize, int seedValue, int octaves, int maxOffsetOctaves,
        int minOffsetOctaves, Vector2 offset, float persistence, float lacunarity, float scale = 0.1f)
    {
        if(octaves < 0)
        {
            octaves = 0;
        }
        System.Random rnd = new System.Random(seedValue);
        Vector2[] octaveOffset = new Vector2[octaves];
        float HalfSize = mapSize / 2.0f;
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rnd.Next(minOffsetOctaves, maxOffsetOctaves) + offset.x;
            float offsetY = rnd.Next(minOffsetOctaves, maxOffsetOctaves) + offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
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
                float amplitude = 1;
                float frequency = 1;
                float noiseMapHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float samepleX = ((HalfSize - x) / scale * frequency) + octaveOffset[i].x;
                    float samepleY = ((HalfSize - y) / scale * frequency) + octaveOffset[i].y;

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
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseMapHeight, maxNoiseMapHeight, noiseMap[x, y]);
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
                    if (assets[i].HeightInMap >= noiseMap[x, y])
                    {
                        color[y * noiseMapHeight + x] = assets[i].color;
                        break;
                    }
                }
            }
        }
        texture2D.SetPixels(color);
        texture2D.Apply();
        return texture2D;
    }

    public static MapGenerator.MeshAssets GenerateMeshAssestsFromNoiseMap(float[,] noiseMap)
    {
        int noiseMapWidth = noiseMap.GetLength(0);
        int noiseMapHeight = noiseMap.GetLength(1);
        
        MapGenerator.MeshAssets meshAssets = new MapGenerator.MeshAssets();
        meshAssets.vertices = new Vector3[noiseMapWidth * noiseMapHeight];
        meshAssets.triangles = new int[(noiseMapWidth - 1) * (noiseMapHeight - 1) * 6];
        meshAssets.uvs = new Vector2[noiseMapWidth * noiseMapHeight];
        float topLeftX = (noiseMapWidth - 1) / -2f;
        float topLeftZ = (noiseMapWidth - 1) / 2f;
        int indexVertices = 0;

        for (int y = 0; y < noiseMapHeight; y++)
        {
            for (int x = 0; x < noiseMapWidth; x++)
            {
                meshAssets.vertices[indexVertices] = new Vector3(topLeftX + x, noiseMap[x, y], topLeftZ - y);
                meshAssets.uvs[indexVertices] = new Vector2(x / (float)noiseMapWidth, y / (float)noiseMapHeight);
                if (x < noiseMapWidth - 1 && y < noiseMapHeight - 1)
                {
                    meshAssets.AddTriangles(indexVertices, indexVertices + noiseMapWidth + 1, indexVertices + noiseMapWidth);
                    meshAssets.AddTriangles(indexVertices + noiseMapWidth + 1, indexVertices, indexVertices + 1);
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
