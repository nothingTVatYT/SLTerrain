using System;
using FlaxEngine;

namespace SLTerrain;

// from: https://de.wikipedia.org/wiki/Perlin-Noise#:~:text=3%20Programmierung-,Definition,eine%20glatte%20Funktion%20zu%20erhalten.
/// <summary>
/// Simple two-dimensional Perlin noise generator
/// </summary>
public class Perlin
{
    private static float Interpolate(float a0, float a1, float x)
    {
        float g;
        g = (3f - x * 2f) * x * x;
        return (a1 - a0) * g + a0;
    }

    private Float2 RandomGradient(int ix, int iy)
    {
        const int w = 8 * sizeof(uint);
        const int s = w / 2;
        uint a = (uint)ix, b = (uint)iy;
        a *= 3284157443;
        b ^= a << s | a >> w-s;
        b *= 1911520717;
        a ^= b << s | b >> w-s;
        a *= 2048419325;
        var random = a * (3.14159265f / ~(~0u >> 1)); // Erzeugt eine Zufallszahl im Intervall [0, 2 * Pi]
        return new Float2(Mathf.Cos(random), Mathf.Sin(random));
    }

    private float DotGridGradient(int ix, int iy, float x, float y)
    {
        var grad = RandomGradient(ix, iy);
        var dx = x - ix;
        var dy = y - iy;
        return dx * grad.X + dy * grad.Y;
    }

    /// <summary>
    /// Generate a value of Perlin noise at x/y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>a value between -1/sqrt(2) and 1/sqrt(2)</returns>
    public float Value(float x, float y)
    {
        // Bestimme die Koordinaten der vier Ecken der Gitterzelle:
        var x0 = Mathf.FloorToInt(x);
        var x1 = x0 + 1;
        var y0 = Mathf.FloorToInt(y);
        var y1 = y0 + 1;

        // Bestimme die Abstände von den Gitterpunkten für die Interpolation:
        var sx = x - x0;
        var sy = y - y0;

        // Interpoliere zwischen den Skalarprodukten an den vier Ecken:
        var n0 = DotGridGradient(x0, y0, x, y);
        var n1 = DotGridGradient(x1, y0, x, y);
        var ix0 = Interpolate(n0, n1, sx);
        n0 = DotGridGradient(x0, y1, x, y);
        n1 = DotGridGradient(x1, y1, x, y);
        var ix1 = Interpolate(n0, n1, sx);

        return Interpolate(ix0, ix1, sy);
    }

    /// <summary>
    /// Generate a normalized value of Perlin noise at x/y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>a value between 0 and 1</returns>
    public float NValue(float x, float y)
    {
        var val = Value(x, y);
        return val + 1 / Mathf.Sqrt(2) / (2 * Mathf.Sqrt(2));
    }
}