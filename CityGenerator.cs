using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour {
    public static CityGenerator Instance;
    public List<Vector2> buildings = new();
    [SerializeField] private int gridX;
    [SerializeField] private int gridY;
    [SerializeField] private float gridSize; //The distance between each building
    [SerializeField] private float noiseScale;
    [SerializeField] private float heightMultiplier; //The highest building will be mult+1 floor high

    [Header("Building Prefab References")]
    [SerializeField] private List<GameObject> building_GroundFloor;
    [SerializeField] private List<GameObject> building_UpperFloor;
    [SerializeField] private GameObject building_Roof;
    private float floorHeight = 3; //Height of each floor object
    public List<GameObject> buildingList = new List<GameObject>();

    private PerlinNoise perlinNoise;
    float shapeOffset = 1f; //This value is used for create a different set of perlin noise for ground floors

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        perlinNoise = new();
    }

    //Generates height for the buildings, might need fixes on the new perlin noise class
    public void GenerateHeight() {
        foreach (Vector2 key in buildings) {
            // float noiseValue = perlinNoise.GenerateNoise(key.x * noiseScale, key.y * noiseScale);
            float noiseValue = perlinNoise.GenerateNoise(key.x, key.y, noiseScale);
            Debug.Log((key.x, key.y, noiseValue));
            //Multiply the noiseValue with a height and round it to floor count
            float height = noiseValue * heightMultiplier + 1;
            int floor = Mathf.RoundToInt(height);
            PlaceBuilding(key, floor);
        }
    }

    //The main function, generates a building with given grid location, and how many floors it have.
    private void PlaceBuilding(Vector2 location, int floorCount) {
        List<GameObject> floors = new List<GameObject>();
        float noiseValue = Mathf.PerlinNoise((location.x + shapeOffset) * noiseScale, (location.y + shapeOffset) * noiseScale);
        for (int i = 0; i < floorCount; i++) {
            GameObject floor;
            switch (i) {
                case 0:
                    //Randomize the ground floor, making each variant around 20% with total of 5 models
                    floor = Instantiate(building_GroundFloor[(int)Mathf.Clamp((noiseValue * 5) - 1, 0, building_GroundFloor.Count-1)]); 
                    floors.Add(floor);
                    break;
                default:
                    //Randomize the upper floors by a new set of perlin noise by calling a new perlin noise every floor
                    float floorNoise = Mathf.PerlinNoise(i * noiseScale * shapeOffset, i * noiseScale * floorCount);
                    floor = Instantiate(building_UpperFloor[(int)Mathf.Clamp((floorNoise * 7) - 1, 0, building_UpperFloor.Count-1)]); 
                    floors.Add(floor);
                    break;  
            }
            //Relocate the floor to the place it should be built
            floor.transform.position = new Vector3(location.x * gridSize, i * floorHeight, location.y * gridSize);
        }
        //Adding a roof to the buildings
        GameObject roof = Instantiate(building_Roof);
        roof.transform.position = new Vector3(location.x * gridSize, floorCount * floorHeight, location.y * gridSize);
        // Debug.Log($"Building at {location} is {floorCount} stories tall");

        floors.Add(roof);
        
        // Create a new building object and add the floors to it
        GameObject building = new GameObject("Building");
        foreach (GameObject floor in floors) {
            floor.transform.parent = building.transform;
            floor.transform.localPosition = new Vector3(0, floor.transform.localPosition.y, 0);
        }
        buildingList.Add(building);
        building.transform.position = new Vector3(location.x, 0, location.y);
    }

    public void AddBuilding(Vector2 location) => buildings.Add(location);
}