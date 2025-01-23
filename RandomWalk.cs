using System.Collections.Generic;
using UnityEngine;

public class Road {
    public Vector3 startPosition, endPosition;
    public Road nextRoad;
    public Road(Vector3 startPosition, Vector3 endPosition, Road nextRoad) {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.nextRoad = nextRoad;
    }
}

public class EdgePoint {
    public int x;
    public int y;
    public int direction;
    public int reverseDirection;
    public List<EdgePoint> borderPoints = new List<EdgePoint>();
}

public enum MeshType {
    Road,
    CrossRoad
}

public class RandomWalk : MonoBehaviour {
    public static RandomWalk Instance;
    public int width = 1024;
    public int height = 1024;
    public int roadLength = 4;
    public int roadPrefabLength = 1;
    public int roadWidth = 1;
    public int steps = 10;
    public int seed = -1;
    public float thirdBranchPossibility = 0.25f;
    public float secondBranchPossibility = 0.75f;
    public float densityOnCenter = 0.5f;
    public float noiseScale = 0.01f;
    public float smallPropsPossibility = 0.5f;
    public float mediumPropsPossibility = 0.25f;
    public float hugePropsPossibility = 0.1f;
    public int propsLimit = 3;
    bool [,] roadMap;
    bool [,] crossRoadMap;
    public List<Transform> crossWalkPoints = new List<Transform>();
    public List<EdgePoint> allEdgePoints = new List<EdgePoint>();
    public List<EdgePoint> edgePoints = new List<EdgePoint>();
    public List<EdgePoint> nextEdgePoints = new List<EdgePoint>();
    public List<GameObject> anyObject = new List<GameObject>();
    public List<Vector3> buildingLocations = new List<Vector3>();
    public List<Quaternion> buildingRotations = new List<Quaternion>();
    public Material material;
    public Material crossRoadMaterial;
    public GameObject roadPrefab, centerRoadPrefab, sideWalkPrefab, crossWalkPrefab;
    public GameObject trafficLightPrefab, lampPrefab, treePrefab, flowerPrefab;
    public GameObject plantPrefab, sidewalkPolePrefab, stopSignPrefab, bigTrashPrefab, smallTrashPrefab;

    public List<GameObject> hugePrefabs;
    public List<GameObject> mediumPrefabs;
    public List<GameObject> smallPrefabs;

    private void Awake() {
        // Set the instance
        Instance = this;

        // set seed for random
        if (seed != -1) Random.InitState(seed);
    }

    private void Start() {
        // Generate the city
        GenerateRoads();
    }

    public void ClearRoads() {
        foreach (Transform child in transform) Destroy(child.gameObject);
        crossWalkPoints.Clear();
        allEdgePoints.Clear();
        edgePoints.Clear();
        nextEdgePoints.Clear();
        roadMap = new bool[width, height];
        crossRoadMap = new bool[width, height];
        buildingLocations.Clear();
        buildingRotations.Clear();
        foreach (GameObject buildingGameObject in CityGenerator.Instance.buildingList) 
            if (buildingGameObject != null) Destroy(buildingGameObject);
        CityGenerator.Instance.buildingList = new List<GameObject>();
        CityGenerator.Instance.buildings = new List<Vector2>();
        GenerateRandomAgents.Instance.RemoveAgents();
    }

    public void GenerateRoads() {
        // Clear the roads
        ClearRoads();
        // Create a new road map
        roadMap = new bool[width, height];
        // Create a new cross road map
        crossRoadMap = new bool[width, height];
        // Set the center of the map as the starting point
        int x = width / 2;
        int y = height / 2;
        roadMap[x, y] = true;
        PerlinNoise perlinNoise = new PerlinNoise();
        // Set edge points
        edgePoints.Add(new EdgePoint { x = x, y = y });
        // Instantiate center road
        Instantiate(centerRoadPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
        // Add the edge point to the all edge points
        allEdgePoints.Add(edgePoints[0]);
        // Set the reverse direction
        edgePoints[0].reverseDirection = -1;
        // Create a new road
        for (int i = 0; i < steps; i++) {
            for (int j = 0; j < edgePoints.Count; j++) {
                // Generate branch random using perlin noise
                float branchRandom = perlinNoise.GenerateNoise(edgePoints[j].x, edgePoints[j].y, noiseScale);
                // Generate branch count
                int branchCount = branchRandom < densityOnCenter * (1 - (1 - thirdBranchPossibility) * (1 - (i / steps))) ? 3 : branchRandom < densityOnCenter * (1 - (1 - secondBranchPossibility) * (1 - (i / steps))) ? 2 : 1;
                // Generate directions available
                bool[] directionsAvailable = new bool[4];
                // Generate a road for each direction that is not backwards
                if(edgePoints[j].reverseDirection != -1) directionsAvailable[edgePoints[j].reverseDirection] = true;
                // Generate a road for each branch
                for (int k = 0; k < branchCount; k++) {
                    x = edgePoints[j].x;
                    y = edgePoints[j].y;
                    // Initialize a new directions array with -1
                    List<int> directions = new List<int>();
                    // Look to each available direction
                    for (int l = 0; l < 4; l++) {
                        // direction is available add it to the directions array
                        if (!directionsAvailable[l]) directions.Add(l);
                    }
                    // If there are no directions available continue
                    if (directions.Count == 0) continue;
                    // Generate a random direction
                    int randomIndex = Random.Range(0, directions.Count);
                    // Get the random direction
                    int randomDirection = directions[randomIndex];
                    // Set prop counts
                    int smallPropsCount = 0;
                    int mediumPropsCount = 0;
                    int hugePropsCount = 0;
                    for (int m = 0; m < roadLength; m++) {
                        switch (randomDirection) {
                            case 0:
                                x += roadPrefabLength;
                                break;
                            case 1:
                                x -= roadPrefabLength;
                                break;
                            case 2:
                                y += roadPrefabLength;
                                break;
                            case 3:
                                y -= roadPrefabLength;
                                break;
                        }

                        if (m != roadLength - 1) {
                            if (x >= 0 && x < width && y >= 0 && y < height) {
                                roadMap[x, y] = true;
                                // Instantiate road prefab
                                if (m == 0 || m == roadLength - 2) {
                                    bool isCrossWalkSet = false;
                                    if (randomDirection == 0) {
                                        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                        //cube.name = "Cross Walk Point";
                                        //cube.transform.position = new Vector3(x, 0.1f, y);
                                        foreach (Transform crossWalk in crossWalkPoints) if (crossWalk.position == new Vector3(x, 0.001f, y)) isCrossWalkSet = true;
                                        if (!isCrossWalkSet) {
                                            GameObject crossWalkGameObject = Instantiate(crossWalkPrefab, new Vector3(x, 0.001f, y), Quaternion.Euler(0, 0, 0), transform);
                                            crossWalkPoints.Add(crossWalkGameObject.transform);
                                        }
                                    }
                                    else if (randomDirection == 1) {
                                        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                        //cube.name = "Cross Walk Point 2";
                                        //cube.transform.position = new Vector3(x, 0.1f, y);
                                        foreach (Transform crossWalk in crossWalkPoints) if (crossWalk.position == new Vector3(x, 0.001f, y)) isCrossWalkSet = true;
                                        if (!isCrossWalkSet) {
                                            GameObject crossWalkGameObject = Instantiate(crossWalkPrefab, new Vector3(x, 0.001f, y), Quaternion.Euler(0, 0, 0), transform);
                                            crossWalkPoints.Add(crossWalkGameObject.transform);
                                        }
                                    }
                                    else if (randomDirection == 2) {
                                        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                        //cube.name = "Cross Walk Poin 3t";
                                        //cube.transform.position = new Vector3(x, 0.1f, y);
                                        foreach (Transform crossWalk in crossWalkPoints) if (crossWalk.position == new Vector3(x, 0.001f, y - 5)) isCrossWalkSet = true;
                                        if (!isCrossWalkSet) {
                                            GameObject crossWalkGameObject = Instantiate(crossWalkPrefab, new Vector3(x, 0.001f, y - 5), Quaternion.Euler(0, 90, 0), transform);
                                            crossWalkPoints.Add(crossWalkGameObject.transform);
                                        }
                                    }
                                    else if (randomDirection == 3) {
                                        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                        //cube.name = "Cross Walk Point 4";
                                        //cube.transform.position = new Vector3(x, 0.1f, y);
                                        foreach (Transform crossWalk in crossWalkPoints) if (crossWalk.position == new Vector3(x - 5, 0.001f, y)) isCrossWalkSet = true;
                                        if (!isCrossWalkSet) {
                                            GameObject crossWalkGameObject = Instantiate(crossWalkPrefab, new Vector3(x - 5, 0.001f, y), Quaternion.Euler(0, 270, 0), transform);
                                            crossWalkPoints.Add(crossWalkGameObject.transform);
                                        }
                                    }
                                }
                                else {
                                    if (randomDirection > 1) Instantiate(roadPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                                    else Instantiate(roadPrefab, new Vector3(x, 0, y - 5), Quaternion.Euler(0, 90, 0), transform);
                                }

                                GameObject sideWalkOne, sideWalkTwo;

                                // Instantiate side walk prefabs
                                if (randomDirection > 1) {
                                    sideWalkOne = Instantiate(sideWalkPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, -90, 0), transform);
                                    sideWalkTwo = Instantiate(sideWalkPrefab, new Vector3(x - 5, 0, y-5), Quaternion.Euler(0, 90, 0), transform);
                                    
                                    Vector3 buildingOnePosition = sideWalkOne.transform.position;
                                    Quaternion buildingOneRotation = sideWalkOne.transform.rotation;
                                    Vector3 buildingTwoPosition = sideWalkTwo.transform.position;
                                    Quaternion buildingTwoRotation = sideWalkTwo.transform.rotation;
                                    buildingLocations.Add(buildingOnePosition);
                                    buildingRotations.Add(buildingOneRotation);
                                    buildingLocations.Add(buildingTwoPosition);
                                    buildingRotations.Add(buildingTwoRotation);
                                }
                                else {
                                    sideWalkOne = Instantiate(sideWalkPrefab, new Vector3(x, 0, y - 5), Quaternion.identity, transform);
                                    sideWalkTwo = Instantiate(sideWalkPrefab, new Vector3(x - 5, 0, y), Quaternion.Euler(0, -180, 0), transform);
                                    
                                    Vector3 buildingOnePosition = sideWalkOne.transform.position;
                                    Quaternion buildingOneRotation = sideWalkOne.transform.rotation;
                                    Vector3 buildingTwoPosition = sideWalkTwo.transform.position;
                                    Quaternion buildingTwoRotation = sideWalkTwo.transform.rotation;
                                    buildingLocations.Add(buildingOnePosition);
                                    buildingRotations.Add(buildingOneRotation);
                                    buildingLocations.Add(buildingTwoPosition);
                                    buildingRotations.Add(buildingTwoRotation);
                                }

                                // Add lamps
                                if (m == roadLength / 2) {
                                    GameObject lampOne = Instantiate(lampPrefab, sideWalkOne.transform.position, sideWalkOne.transform.rotation, transform);
                                    // move object to 4 units backwards in transform forward
                                    lampOne.transform.Translate(Vector3.forward * -4);
                                    GameObject lampTwo = Instantiate(lampPrefab, sideWalkTwo.transform.position, sideWalkTwo.transform.rotation, transform);
                                    // move object to 4 units backwards in transform forward
                                    lampTwo.transform.Translate(Vector3.forward * -4);
                                }

                                if (m < roadLength - 2 && m > 1 && m != roadLength / 2) {
                                    // Add props
                                    float propRandom = Random.Range(0, 1f);

                                    if (propRandom < hugePropsPossibility && hugePropsCount < propsLimit) {
                                        hugePropsCount++;
                                        GameObject hugeProp = Instantiate(hugePrefabs[Random.Range(0, hugePrefabs.Count)], sideWalkOne.transform.position, sideWalkOne.transform.rotation, transform);
                                        hugeProp.transform.Translate(Vector3.forward * -4);
                                    }
                                    else if (propRandom < mediumPropsPossibility && mediumPropsCount < 1) {
                                        mediumPropsCount++;
                                        GameObject mediumProp = Instantiate(mediumPrefabs[Random.Range(0, mediumPrefabs.Count)], sideWalkOne.transform.position, sideWalkOne.transform.rotation, transform);
                                        mediumProp.transform.Translate(Vector3.forward * -2);
                                    }
                                    else if (propRandom < smallPropsPossibility) {
                                        smallPropsCount++;
                                        GameObject smallProp = Instantiate(smallPrefabs[Random.Range(0, smallPrefabs.Count)], sideWalkOne.transform.position, sideWalkOne.transform.rotation, transform);
                                        smallProp.transform.Translate(Vector3.forward * -1);
                                    }
                                }
                            }
                        }
                    }

                    // Check if the road is within the map
                    if (x >= 0 && x < width && y >= 0 && y < height) {
                        // Check if the road is already set
                        if (roadMap[x, y]) continue;
                        // Set the road in the road map
                        roadMap[x, y] = true;
                        // Instantiate center road prefab
                        if (randomDirection > 1) Instantiate(centerRoadPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                        else Instantiate(centerRoadPrefab, new Vector3(x, 0, y - 5), Quaternion.Euler(0, 90, 0), transform);
                        // Add the edge point to the next edge points
                        nextEdgePoints.Add(new EdgePoint { 
                            x = x, 
                            y = y,
                            direction = randomDirection,
                            reverseDirection = randomDirection == 0 ? 1 : randomDirection == 1 ? 0 : randomDirection == 2 ? 3 : 2
                        });
                        // Add the edge point to the all edge points
                        allEdgePoints.Add(nextEdgePoints[nextEdgePoints.Count - 1]);
                        // Add the edge point to the border points of the current edge point
                        edgePoints[j].borderPoints.Add(nextEdgePoints[nextEdgePoints.Count - 1]);
                    }
                }
            }
            edgePoints.Clear();
            foreach (EdgePoint edgePoint in nextEdgePoints) edgePoints.Add(edgePoint);
            nextEdgePoints.Clear();
        }

        // Close the roads
        foreach (EdgePoint edgePoint in allEdgePoints) {
            if (
                edgePoint.x + 5 >= 0 && 
                edgePoint.x <= width && 
                edgePoint.y >= 0 && 
                edgePoint.y <= height && 
                !roadMap[edgePoint.x + 5, edgePoint.y]
            ) {
                // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // cube.name = "Empty Point";
                // cube.transform.position = new Vector3(edgePoint.x + 5, 0, edgePoint.y);
                GameObject sideWalk = Instantiate(sideWalkPrefab, new Vector3(edgePoint.x, 0, edgePoint.y), Quaternion.Euler(0, -90, 0), transform);
                Vector3 buildingOnePosition = sideWalk.transform.position;
                Quaternion buildingOneRotation = sideWalk.transform.rotation;
                buildingLocations.Add(buildingOnePosition);
                buildingRotations.Add(buildingOneRotation);
            }
            if (
                edgePoint.x - 5 >= 0 && 
                edgePoint.x <= width && 
                edgePoint.y >= 0 && 
                edgePoint.y <= height && 
                !roadMap[edgePoint.x - 5, edgePoint.y]
            ) {
                // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // cube.name = "Empty 2 Point";
                // cube.transform.position = new Vector3(edgePoint.x - 5, 0, edgePoint.y);
                GameObject sideWalk = Instantiate(sideWalkPrefab, new Vector3(edgePoint.x - 5, 0, edgePoint.y - 5), Quaternion.Euler(0, 90, 0), transform);
                Vector3 buildingOnePosition = sideWalk.transform.position;
                Quaternion buildingOneRotation = sideWalk.transform.rotation;
                buildingLocations.Add(buildingOnePosition);
                buildingRotations.Add(buildingOneRotation);
            }
            if (
                edgePoint.x >= 0 && 
                edgePoint.x <= width && 
                edgePoint.y + 5 >= 0 && 
                edgePoint.y <= height && 
                !roadMap[edgePoint.x, edgePoint.y + 5]
            ) {
                // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // cube.name = "Empty 3 Point";
                // cube.transform.position = new Vector3(edgePoint.x, 0, edgePoint.y + 5);
                GameObject sideWalk = Instantiate(sideWalkPrefab, new Vector3(edgePoint.x - 5, 0, edgePoint.y), Quaternion.Euler(0, 180, 0), transform);
                Vector3 buildingOnePosition = sideWalk.transform.position;
                Quaternion buildingOneRotation = sideWalk.transform.rotation;
                buildingLocations.Add(buildingOnePosition);
                buildingRotations.Add(buildingOneRotation);
            }
            if (
                edgePoint.x >= 0 && 
                edgePoint.x <= width && 
                edgePoint.y - 5 >= 0 && 
                edgePoint.y <= height && 
                !roadMap[edgePoint.x, edgePoint.y - 5]
            ) {
                // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // cube.name = "Empty 4 Point";
                // cube.transform.position = new Vector3(edgePoint.x, 0, edgePoint.y - 5);
                GameObject sideWalk = Instantiate(sideWalkPrefab, new Vector3(edgePoint.x, 0, edgePoint.y - 5), Quaternion.identity, transform);
                Vector3 buildingOnePosition = sideWalk.transform.position;
                Quaternion buildingOneRotation = sideWalk.transform.rotation;
                buildingLocations.Add(buildingOnePosition);
                buildingRotations.Add(buildingOneRotation);
            }
        }

        // Look to each road adjacent places for building placement
        // CityGenerator.Instance.AddBuilding(new Vector2(0, 0));

        foreach (Vector3 buildingLocation in buildingLocations) {
            CityGenerator.Instance.AddBuilding(new Vector2(buildingLocation.x, buildingLocation.z));
        }

        // Generate the city
        CityGenerator.Instance.GenerateHeight();
        
        int index = 0;
        foreach (GameObject buildingGameObject in CityGenerator.Instance.buildingList) {
            if (index >= buildingRotations.Count) break;

            GameObject left = new GameObject("Building Left");
            left.AddComponent<BoxCollider>();
            left.gameObject.tag = "Building";
            left.transform.SetParent(buildingGameObject.transform);
            left.transform.localPosition = Vector3.zero;
            left.AddComponent<Rigidbody>();
            left.GetComponent<Rigidbody>().isKinematic = true;

            GameObject front = new GameObject("Building Front");
            front.transform.SetParent(buildingGameObject.transform);
            front.transform.localPosition = Vector3.zero;
            // Move 2.5f right
            front.transform.Translate(Vector3.right * -2.5f);

            GameObject right = new GameObject("Building Right");
            right.AddComponent<BoxCollider>();
            right.gameObject.tag = "Building";
            right.transform.SetParent(buildingGameObject.transform);
            right.transform.localPosition = Vector3.zero;
            right.AddComponent<Rigidbody>();
            right.GetComponent<Rigidbody>().isKinematic = true;
            // move 5f right
            right.transform.Translate(Vector3.right * -5f);

            buildingGameObject.transform.rotation = buildingRotations[index];
            buildingGameObject.transform.Translate(Vector3.forward * -5);
            index++;

            if (roadMap[(int) right.transform.position.x, (int) right.transform.position.z + 5]) Destroy(buildingGameObject);
        }
        
        // Generate navmesh for the city
        NavmeshSurfaceGenerator.Instance.GenerateNavmesh();

        // Generate agents
        GenerateRandomAgents.Instance.CreateAgents();
    }

    public Vector3 GetRandomPosition() {
        // Get a random edge point from all edge points
        EdgePoint edgePoint = allEdgePoints[Random.Range(0, allEdgePoints.Count)];
        // Get x and y from the edge point
        int x = edgePoint.x;
        int y = edgePoint.y;
        return new Vector3(x, 0, y);
    }
}