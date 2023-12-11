using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 3.0f;

    private Transform _transform;
    private float distance = 10.0f;

    private Transform _targetTransform;

    private float xAngle = 0.0f;
    private float yAngle = 0.0f;

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

        Vector3 offset = _targetTransform.position - _transform.position;
        distance = offset.magnitude;
    }

    void LateUpdate()
    {
        float horizontalInput = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float verticalInput = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        Vector3 targetPosition = _targetTransform.position;

#if false
        Vector3 offset = _transform.position - targetPosition;

        verticalInput *= Vector3.Dot(transform.right, _targetTransform.right) > 0 ? -1 : 1;

        // horizontalInput = Math.Clamp(horizontalInput, -90, 90);
        // verticalInput = Math.Clamp(verticalInput, -90, 90);

        Debug.Log(verticalInput + "," + _transform.rotation.eulerAngles.x);

        Quaternion rotation = Quaternion.Euler(verticalInput, horizontalInput, 0);
        Vector3 newPosition = targetPosition + (rotation * offset).normalized * distance;
#else
        xAngle += verticalInput;
        yAngle += horizontalInput;

        xAngle = Math.Clamp(xAngle, -89, 89);

        Quaternion rotation = Quaternion.Euler(xAngle, yAngle, 0);
        Vector3 newPosition = targetPosition + (rotation * Vector3.forward).normalized * distance;
        
#endif
        _transform.position = newPosition;
        _transform.LookAt(targetPosition);
    }
}