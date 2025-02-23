using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise
{
    // Static permutation table for perlin noise
    // Hash table retrieved from https://mrl.cs.nyu.edu/~perlin/noise/ (Original perlin noise)
    private int[] permutation = {151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
        8,99,37,240,21,10,23,190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,77,146,158,231,83,111,
        229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,102,143,54, 65,25,63,161,1,216,80,73,209,
        76,132,187,208,89,18,169,200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,
        250,124,123,5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,223,183,
        170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,129,22,39,253,19,98,108,110,
        79,113,224,232,178,185,112,104,218,246,97,228,251,34,242,193,238,210,144,12,191,179,162,241,81,
        51,145,235,249,14,239,107,49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,
        254,138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
    };

    // Get hashed value generated for the x, y position
    // private int GetHashValue(int x, int y) => permutation[((int)Mathf.Floor(x) & 255) + permutation[(int)Mathf.Floor(y) & 255]];

    private int GetHashValue(int x, int y) => permutation[(permutation[x % 256] + y % 256) % 256];

    // Fade function for smooth interpolation, S(t) = 6t^5 - 15t^4 + 10t^3
    private float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

    // Linear interpolation function, Lerp(a, b, t) = a + t(b - a)
    private float Lerp(float a, float b, float t) => a + t * (b - a);

    // Generate a new gradient vector for the given hash value
    private float Gradient(int hash, float x, float y) {
        // Get the hash value for the gradient
        int h = hash & 15;
        // u and v are the gradient vectors
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
        // Return the scalar value generated from the gradient vector
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    private int[] GetCornerPoints(int x, int y) {
        // Get the hash value for the four corner points
        int upperLeft = GetHashValue(x, y + 1);
        int upperRight = GetHashValue(x + 1, y + 1);
        int lowerLeft = GetHashValue(x, y);
        int lowerRight = GetHashValue(x + 1, y);
        return new int[] {upperLeft, upperRight, lowerLeft, lowerRight};
    }

    public float GenerateNoise(float x, float y, float scale) {
        x *= scale;
        y *= scale;
        // Generate square grid around the given x, y position
        int xPos = (int) Mathf.Floor(x) & 255;
        int yPos = (int) Mathf.Floor(y) & 255;
        // Get the fractional part of the x, y position for using it on perlin noise
        // Fractional part is required for smooth interpolation (Corners weight is calculated based on this)
        float xFraction = x - Mathf.Floor(x);
        float yFraction = y - Mathf.Floor(y);
        // Calculate the corner points
        int[] cornerPoints = GetCornerPoints(xPos, yPos);
        // Calculate the fade value for the fractional part
        float u = Fade(xFraction);
        float v = Fade(yFraction);
        // Interpolate the values for the x-axis
        float upper = Lerp(Gradient(cornerPoints[0], xFraction, yFraction), Gradient(cornerPoints[1], xFraction - 1, yFraction), u);
        float lower = Lerp(Gradient(cornerPoints[2], xFraction, yFraction - 1), Gradient(cornerPoints[3], xFraction - 1, yFraction - 1), u);
        // Interpolate the values for the y-axis
        float value = Lerp(upper, lower, v);
        return (value + 1) / 2;
    }
}
