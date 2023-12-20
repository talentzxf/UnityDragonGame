using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

public class PlayerGroundMovement : StateBehaviour
{
    private CharacterController _cc;
    private NetworkMecanimAnimator _networkAnimator;

    private Transform _transform;

    private float maxSpeed = 2f;
    public float rotationSpeed = 10.0f;

    private Camera camera;

    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _cc = GetComponentInChildren<CharacterController>();

            _networkAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
            _transform = _cc.transform;
            
            camera = Camera.main;   
            FirstPersonCamera fpsCamera = camera.GetComponent<FirstPersonCamera>();
            fpsCamera.SetCameraTarget(_cc.gameObject);
        }
    }
    
    protected override void OnFixedUpdate()
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
            _networkAnimator.Animator.SetBool(_isIdleHash, true);
        }
        else
        {
            _networkAnimator.Animator.SetBool(_isIdleHash, false);

            // Lerp rotate the character.
            Quaternion targetRotation = Quaternion.LookRotation(resultVelocityXY, Vector3.up);
            Quaternion resultRotation =
                Quaternion.Slerp(_transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);

            // Keep the character always up.
            resultRotation.x = 0;
            resultRotation.z = 0;
            _transform.rotation = resultRotation;
        }

        _networkAnimator.Animator.SetFloat(_speed, speed);
    }
}