using UnityEngine;
using UnityEngine.UI;

public class PerlinNoiseRenderer : MonoBehaviour
{
    public int width;
    public int height;
    public RawImage rawImage; // Reference to the RawImage UI component
    private Texture2D noiseTexture;
    private PerlinNoise perlin = new PerlinNoise();
    public static PerlinNoiseRenderer Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateNoiseTexture();
    }

    public void GenerateNoiseTexture()
    {
        width = RandomWalk.Instance.width;
        height = RandomWalk.Instance.height;
        Debug.Log("Generating noise texture with width: " + width + " and height: " + height);
        noiseTexture = new Texture2D(width, height);
        rawImage.texture = noiseTexture;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = perlin.GenerateNoise(x, y, RandomWalk.Instance.noiseScale);
                noiseTexture.SetPixel(x, y, new Color(value, value, value));
            }
        }
        noiseTexture.Apply();
    }
}