using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform cameraTransform;

    public float speed = 10f, rotationSpeed = 100f;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        // Set camera position to city center
        cameraTransform.position = new Vector3(RandomWalk.Instance.width / 2, 20, RandomWalk.Instance.height / 2);
    }

    private void Update()
    {
        if (UIManager.Instance.backgroundImage.activeSelf) return;
        
        // Get horizontal and vertical input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Get mouse rotation input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Update camera position based on input with speed and deltaTime
        cameraTransform.position += cameraTransform.forward * verticalInput * speed * Time.deltaTime;
        cameraTransform.position += cameraTransform.right * horizontalInput * speed * Time.deltaTime;

        // Update camera rotation based on mouse input with speed and deltaTime
        cameraTransform.eulerAngles += new Vector3(-mouseY, mouseX, 0) * rotationSpeed * Time.deltaTime;
    }
}