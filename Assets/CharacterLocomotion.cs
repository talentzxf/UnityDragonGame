using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10.0f;
    [SerializeField] private String dragonNeckName = "Bone.008";
    [SerializeField] private String leftFootName = "LeftFoot";
    [SerializeField] private String rightFootName = "RightFoot";
    [SerializeField] private String sitPoint = "SitPoint";

    private Animator _animator;
    private Transform _camera;
    private CharacterController _character;

    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");
    private int _climb = Animator.StringToHash("climb");
    private int _climbdown = Animator.StringToHash("climbDown");
    private Transform _mainCameraTransform;

    private bool isClimbing = false;
    private bool isOnDragon;

    private Transform _leftFootIK;
    private Transform _rightFootIK;

    Transform GetLeftFootIK()
    {
        if (_leftFootIK == null)
        {
            _leftFootIK = Utility.RecursiveFind(dragonTransform, leftFootName);
        }

        return _leftFootIK;
    }

    Transform GetRightFootIK()
    {
        if (_rightFootIK == null)
        {
            _rightFootIK = Utility.RecursiveFind(dragonTransform, rightFootName);
        }

        return _rightFootIK;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        float climbUpProgress = _animator.GetFloat("ClimbUpProgress");
        if (climbUpProgress > 0.5)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, climbUpProgress);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, climbUpProgress);
            
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, GetLeftFootIK().position);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, GetRightFootIK().position);            
        }
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _mainCameraTransform = Camera.main.transform;
        _character = GetComponent<CharacterController>();

        if (_animator == null)
        {
            Debug.LogError("Can't find animator!!!");
        }
    }

    private Transform dragonTransform;

    public void Climb(Vector3 startPosition, GameObject dragonGO)
    {
        dragonTransform = dragonGO.transform;
        Vector3 forwardDir = dragonTransform.right;
        isClimbing = true;
        // Align rotation
        transform.position = startPosition;
        transform.forward = forwardDir;

        climbStartForward = forwardDir;

        _character.enabled = false;

        _animator.SetTrigger(_climb);
    }

    private Vector3 climbStartForward;

    IEnumerator FixPlayerPosition(float durationSeconds)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = Utility.RecursiveFind(dragonTransform, sitPoint).position;
        
        float percentage = 0.0f;
        while (percentage < durationSeconds && isClimbing)
        {
            transform.position = Vector3.Slerp(startPosition, endPosition, percentage);
            percentage += Time.deltaTime;
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Clicked M, climbing down");
            _character.enabled = true;
            _animator.SetTrigger(_climbdown);
            isClimbing = false;
            isOnDragon = false;
            transform.SetParent(null);
        }

        if (isClimbing && !isOnDragon)
        {
            float climbUpProgress = _animator.GetFloat("ClimbUpProgress");
            Vector3 currentForward = Vector3.Lerp(climbStartForward, dragonTransform.forward, climbUpProgress);
            transform.forward = currentForward;

            if (climbUpProgress > 0.99f)
            {
                StartCoroutine(FixPlayerPosition(0.5f));
                Transform bone08 = Utility.RecursiveFind(dragonTransform, dragonNeckName);
                transform.SetParent(bone08);
                isOnDragon = true;
            }
        }
        
        if(!isClimbing && !isOnDragon)
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector3 forwardVelocity = vertical * _mainCameraTransform.forward;
            Vector3 horizontalVelocity = horizontal * _mainCameraTransform.right;

            Vector3 resultVelocity = forwardVelocity + horizontalVelocity;

            float speed = resultVelocity.magnitude;

            if (speed < Mathf.Epsilon)
            {
                _animator.SetBool(_isIdleHash, true);
            }
            else
            {
                _animator.SetBool(_isIdleHash, false);

                // Lerp rotate the character.
                Vector3 characterDir = forwardVelocity + horizontalVelocity;


                Quaternion targetRotation = Quaternion.LookRotation(characterDir, Vector3.up);
                Quaternion resultRotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Keep the character always up.
                resultRotation.x = 0;
                resultRotation.z = 0;
                transform.rotation = resultRotation;
            }

            _animator.SetFloat(_speed, speed);
        }
    }
}