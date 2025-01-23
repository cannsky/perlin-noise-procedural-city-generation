using UnityEngine;
using System.Collections.Generic;

public class GenerateRandomAgents : MonoBehaviour
{
    public static GenerateRandomAgents Instance;
    public List<GameObject> agentPrefabs;
    public List<GameObject> agentList;
    public int agentCount = 5;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateAgents()
    {
        agentCount = RandomWalk.Instance.steps / 2;
        for (int i = 0; i < agentCount; i++)
        {
            // Get spawn position
            Vector3 spawnPosition = RandomWalk.Instance.GetRandomPosition();

            // Get random agent prefab
            GameObject agentPrefab = agentPrefabs[Random.Range(0, agentPrefabs.Count)];

            // Instantiate agent
            GameObject agent = Instantiate(agentPrefab, spawnPosition, Quaternion.identity);

            // Add agent to the list
            agentList.Add(agent);
        }
    }

    public void RemoveAgents() {
        foreach (GameObject agent in agentList) {
            Destroy(agent);
        }
        agentList = new List<GameObject>();
    }
}