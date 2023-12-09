using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] Transform target;

    private Transform _transform;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 targetPosition = target.position;
        
        Vector3 offset = _transform.position - targetPosition;
        float horizontalInput = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float verticalInput = -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // horizontalInput = Math.Clamp(horizontalInput, -90, 90);
        // verticalInput = Math.Clamp(verticalInput, -90, 90);
        
        Quaternion rotation = Quaternion.Euler(verticalInput, horizontalInput, 0);
        Debug.Log("verticalInput:" + verticalInput + " hInput:" + horizontalInput);
        Vector3 newPosition = targetPosition + rotation * offset;

        _transform.position = newPosition;
        _transform.LookAt(targetPosition);
    }
}