using System;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Transform target;
    public float MouseSensitivity = 10f;

    private float xAngle;
    private float yAngle;

    private float distance;

    public void SetCameraTarget(GameObject playerGO)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        target = playerGO.transform;
        Animator playerAnimator = playerGO.GetComponentInChildren<Animator>();
        if (playerAnimator && playerAnimator.isHuman)
        {
            target = playerAnimator.GetBoneTransform(HumanBodyBones.Neck);
        }

        Vector3 offset = target.position - transform.position;
        distance = offset.magnitude;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;

        float horizontalInput = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float verticalInput = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        xAngle += verticalInput;
        yAngle += horizontalInput;
        xAngle = Math.Clamp(xAngle, -89, 89);
        Quaternion rotation = Quaternion.Euler(xAngle, yAngle, 0);
        Vector3 newPosition = targetPosition + (rotation * Vector3.forward).normalized * distance;

        transform.position = newPosition;
        transform.LookAt(targetPosition);
    }
}