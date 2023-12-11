using UnityEditor.VersionControl;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 3.0f;
    
    private Transform _transform;
    private float distance = 10.0f;

    private Transform _targetTransform;
    
    private void Start()
    {
        _transform = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameObject player = GameObject.FindWithTag("Player");
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator && playerAnimator.isHuman)
        {
            _targetTransform = playerAnimator.GetBoneTransform(HumanBodyBones.Neck);
        }


        distance = (_targetTransform.position - _transform.position).magnitude;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = _targetTransform.position;
        Vector3 offset = _transform.position - targetPosition;
        
        float horizontalInput = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float verticalInput = -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        // horizontalInput = Math.Clamp(horizontalInput, -90, 90);
        // verticalInput = Math.Clamp(verticalInput, -90, 90);
        
        Quaternion rotation = Quaternion.Euler(verticalInput, horizontalInput, 0);
        Vector3 newPosition = targetPosition + (rotation * offset).normalized * distance;

        _transform.position = newPosition;
        _transform.LookAt(targetPosition);
    }
}