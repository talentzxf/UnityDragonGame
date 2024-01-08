using System;
using System.Collections;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float MouseSensitivity = 10f;
    public bool clampEnabled = true;

    private float xAngle;
    private float yAngle;

    public float distance;
    private float originalDistance;

    public Transform target;
    public Transform newTarget;
    private float currentProgress = 0.0f;

    public void LerpToDistance(float distanceFactor, Transform inNewTarget, float totalTime)
    {
        this.newTarget = inNewTarget;
        Animator playerAnimator = inNewTarget.GetComponentInChildren<Animator>();
        if (playerAnimator && playerAnimator.isHuman)
        {
            this.newTarget = playerAnimator.GetBoneTransform(HumanBodyBones.Neck);
        }

        StartCoroutine(InternalLerpToDistance(distanceFactor, totalTime));
    }

    private IEnumerator InternalLerpToDistance(float distanceFactor, float durationSeconds)
    {
        float startDistance = distance;
        float targetDistance = originalDistance * distanceFactor;
        float usedTime = 0.0f;

        while (usedTime < durationSeconds)
        {
            distance = Mathf.Lerp(startDistance, targetDistance, usedTime / durationSeconds);

            usedTime += Time.deltaTime;

            currentProgress = usedTime / durationSeconds;
            yield return null;
        }

        currentProgress = 0.0f;
        target = newTarget;
        newTarget = null;
    }

    private void OnEnable()
    {
        if (target != null)
            LockCursor();

        if (target != null)
        {
            Vector3 initDir = Vector3.forward;
            Vector3 resultDir = transform.position - target.position;

            Quaternion resultRotation = Quaternion.FromToRotation(initDir, resultDir.normalized);

            xAngle = resultRotation.eulerAngles.x;
            yAngle = resultRotation.eulerAngles.y;

            Quaternion rotation = Quaternion.Euler(xAngle, yAngle, 0);
            Vector3 newPosition = target.position + (rotation * Vector3.forward).normalized * distance;

            Debug.Log("Calculated xAngle:" + xAngle + " yAngle:" + yAngle);
            Debug.Log("Camera error:" + (newPosition - transform.position));
        }
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

    private int _TerrainLayerMask = -1;

    private int TerrainLayerMask
    {
        get
        {
            if (_TerrainLayerMask < 0)
            {
                _TerrainLayerMask = LayerMask.GetMask("Terrain");
            }

            return _TerrainLayerMask;
        }
    }


    void SyncTransform()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;

        if (newTarget != null)
        {
            targetPosition = Vector3.Lerp(target.position, newTarget.position, currentProgress);
        }

        float horizontalInput = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float verticalInput = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        xAngle += verticalInput;
        yAngle += horizontalInput;

        if (clampEnabled)
            xAngle = Math.Clamp(xAngle, -89, 89);

        Quaternion rotation = Quaternion.Euler(xAngle, yAngle, 0);
        Vector3 newPosition;

        if (Physics.Raycast(targetPosition, Vector3.down, out RaycastHit hit, distance * 2.0f,
                TerrainLayerMask) && hit.distance < 0.5f)
        {
            newPosition = hit.point + 0.5f * Vector3.up;
        }
        else
        {
            newPosition = targetPosition + (rotation * Vector3.forward).normalized * distance;
        }

        Debug.DrawLine(targetPosition, newPosition);

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