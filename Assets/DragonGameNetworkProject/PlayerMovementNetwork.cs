using Fusion;
using UnityEngine;

public class PlayerMovementNetwork : NetworkBehaviour
{
    private CharacterController _cc;

    private float maxSpeed = 2f;
    public float rotationSpeed = 10.0f;

    private Camera camera;

    private Animator _animator;

    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            camera = Camera.main;
            camera.GetComponent<FirstPersonCamera>().SetCameraTarget(gameObject);
        }
    }

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            _animator.applyRootMotion = true;
        }
        else
        {
            _animator.applyRootMotion = false;
        }
        
        if (HasStateAuthority == false)
            return;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 forwardVelocity = vertical * camera.transform.forward;
        Vector3 horizontalVelocity = horizontal * camera.transform.right;

        Vector3 resultVelocityXY = forwardVelocity + horizontalVelocity;

        // Vector3 resultVelocity = resultVelocityXY;
        
        // if (!_cc.isGrounded)
        // {
        //     resultVelocity += Vector3.down;
        // }

        // _cc.Move(resultVelocity * Runner.DeltaTime);

        float speed = resultVelocityXY.magnitude;
        if (speed < Mathf.Epsilon)
        {
            _animator.SetBool(_isIdleHash, true);
        }
        else
        {
            _animator.SetBool(_isIdleHash, false);

            // Lerp rotate the character.
            Quaternion targetRotation = Quaternion.LookRotation(resultVelocityXY, Vector3.up);
            Quaternion resultRotation =
                Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);

            // Keep the character always up.
            resultRotation.x = 0;
            resultRotation.z = 0;
            transform.rotation = resultRotation;
        }

        _animator.SetFloat(_speed, speed);
    }
}