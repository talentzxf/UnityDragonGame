using System;
using System.Collections;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Transform target;
    public float MouseSensitivity = 10f;

    private float xAngle;
    private float yAngle;

    private float distance;
    private float originalDistance;
    
    public void LerpToDistance(float distanceFactor, float totalTime)
    {
        StartCoroutine(InternalLerpToDistance(distanceFactor, totalTime));
    }

    private IEnumerator InternalLerpToDistance(float distanceFactor, float durationSeconds)
    {
        float startDistance = distance;
        float targetDistance = originalDistance * distanceFactor;
        float percentage = 0.0f;

        while (percentage < durationSeconds)
        {
            distance = Mathf.Lerp(startDistance, targetDistance, percentage / durationSeconds);
            
            percentage += Time.deltaTime;
            yield return null;
        }
    }

    private void OnEnable()
    {
        LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;        
    }

    public void SetCameraTarget(GameObject playerGO, bool hideCursor = true)
    {
        LockCursor();
        
        target = playerGO.transform;
        Animator playerAnimator = playerGO.GetComponentInChildren<Animator>();
        if (playerAnimator && playerAnimator.isHuman)
        {
            target = playerAnimator.GetBoneTransform(HumanBodyBones.Neck);
        }

        Vector3 offset = target.position - transform.position;
        distance = offset.magnitude;
        originalDistance = distance;
    }

    void SyncTransform()
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

    private void Update()
    {  
        SyncTransform();
    }

    private void LateUpdate()
    {
        SyncTransform();
    }
}