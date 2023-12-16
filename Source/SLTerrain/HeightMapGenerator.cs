using FlaxEngine;
using FlaxEngine.Utilities;

namespace SLTerrain;

public class HeightMapGenerator : Script
{
    public int Seed;
    public bool RandomizeSeed;

    public int NumOctaves = 7;
    public float Persistence = .5f;
    public float Lacunarity = 2;
    public float InitialScale = 2;

    public float[] Generate(int mapSize)
    {
        var map = new float[mapSize * mapSize];
        Seed = RandomizeSeed ? RandomUtil.Random.Next(-10000, 10000) : Seed;
        var prng = new System.Random(Seed);

        var offsets = new Float2[NumOctaves];
        for (var i = 0; i < NumOctaves; i++)
        {
            offsets[i] = new Float2(prng.Next(-1000, 1000), prng.Next(-1000, 1000));
        }

        var minValue = float.MaxValue;
        var maxValue = float.MinValue;

        for (var y = 0; y < mapSize; y++)
        {
            for (var x = 0; x < mapSize; x++)
            {
                float noiseValue = 0;
                var scale = InitialScale;
                float weight = 1;
                for (var i = 0; i < NumOctaves; i++)
                {
                    var p = offsets[i] + new Float2(x / (float)mapSize, y / (float)mapSize) * scale;
                    noiseValue += Noise.PerlinNoise(p) * weight;
                    weight *= Persistence;
                    scale *= Lacunarity;
                }

                map[y * mapSize + x] = noiseValue;
                minValue = Mathf.Min(noiseValue, minValue);
                maxValue = Mathf.Max(noiseValue, maxValue);
            }
        }

        // Normalize
        if (Mathf.Abs(maxValue - minValue) > Mathf.Epsilon)
        {
            for (var i = 0; i < map.Length; i++)
            {
                map[i] = (map[i] - minValue) / (maxValue - minValue);
            }
        }

        return map;
    }
}