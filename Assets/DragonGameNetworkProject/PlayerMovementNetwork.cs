using Fusion;
using UnityEngine;

public class PlayerMovementNetwork : NetworkBehaviour
{
    private CharacterController _cc;

    private float maxSpeed = 2f;
    public float rotationSpeed = 10.0f;

    private Camera camera;

    private NetworkMecanimAnimator _networkAnimator;

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
        _networkAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            _networkAnimator.Animator.applyRootMotion = true;
        }
        else
        {
            _networkAnimator.Animator.applyRootMotion = false;
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
            _networkAnimator.Animator.SetBool("isIdle", true);
        }
        else
        {
            _networkAnimator.Animator.SetBool("isIdle", false);

            // Lerp rotate the character.
            Quaternion targetRotation = Quaternion.LookRotation(resultVelocityXY, Vector3.up);
            Quaternion resultRotation =
                Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);

            // Keep the character always up.
            resultRotation.x = 0;
            resultRotation.z = 0;
            transform.rotation = resultRotation;
        }

        _networkAnimator.Animator.SetFloat(_speed, speed);
    }
}