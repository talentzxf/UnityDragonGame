using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterLocomotion : MonoBehaviour
{
    private float rotationSpeed = 10.0f;

    private Animator _animator;
    private Transform _camera;
    private CharacterController _character;

    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");
    private int _climb = Animator.StringToHash("climb");
    private int _climbdown = Animator.StringToHash("climbDown");
    private Transform _mainCameraTransform;

    private bool isOnDragon = false;

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
        isOnDragon = true;
        // Align rotation
        transform.position = startPosition;
        transform.forward = forwardDir;

        climbStartForward = forwardDir;

        _character.enabled = false;

        _animator.SetTrigger(_climb);
    }

    private Vector3 climbStartForward;
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            _character.enabled = true;
            _animator.SetTrigger(_climbdown);
            isOnDragon = false;
        }

        if (isOnDragon)
        {
            float climbUpProgress = _animator.GetFloat("ClimbUpProgress");
            Vector3 currentForward = Vector3.Lerp(climbStartForward, dragonTransform.forward, climbUpProgress);
            transform.forward = currentForward;

            if (climbUpProgress > 0.99f)
            {
                transform.SetParent(dragonTransform);
            }
            return;
        }

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