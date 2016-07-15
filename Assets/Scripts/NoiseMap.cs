using UnityEngine;
using System.Collections;

public static class NoiseMap
{
    public static float[,] GenerateNoiseMap(int mapSize, float scale = 0.1f)
    {
        float[,] noiseMap = new float[mapSize, mapSize];
        if(scale <= 0.0f)
        {
            scale = 0.1f;
        }
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float samepleX = x / scale;
                float samepleY = y / scale;
                float perlinValue = Mathf.PerlinNoise(samepleX, samepleY);
                noiseMap[x, y] = perlinValue;
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
}
