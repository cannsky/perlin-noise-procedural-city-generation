using UnityEngine;

public class RoadColliderDetector : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Building")) {
            Destroy(other.gameObject.transform.parent.gameObject);
        }
    }
}