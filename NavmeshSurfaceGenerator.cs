using Unity.AI.Navigation;
using UnityEngine;

public class NavmeshSurfaceGenerator : MonoBehaviour
{
    /*
    Generate a global navmesh surface for the entire city
    */

    public static NavmeshSurfaceGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateNavmesh()
    {
        // Get the NavMeshSurface component
        NavMeshSurface navMeshSurface = GetComponent<NavMeshSurface>();

        // Get all objects
        navMeshSurface.collectObjects = CollectObjects.All;

        // Build the navmesh
        navMeshSurface.BuildNavMesh();
    }
}