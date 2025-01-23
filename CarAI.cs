using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class CarAI : MonoBehaviour {

    public bool isDestinationReached = false;

    public NavMeshAgent navMeshAgent;

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetRandomDestination();
    }
    
    public void SetRandomDestination() {
        Vector3 randomPosition = RandomWalk.Instance.GetRandomPosition();
        Vector3 randomDestination = randomPosition;
        navMeshAgent.SetDestination(randomDestination);
        navMeshAgent.isStopped = false;
    }

    private void Update() {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            isDestinationReached = true;
            navMeshAgent.isStopped = true;
            SetRandomDestination();
        }
    }
}