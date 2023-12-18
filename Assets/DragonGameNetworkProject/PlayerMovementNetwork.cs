using System.Globalization;
using Fusion;
using UnityEngine;

public class PlayerMovementNetwork : NetworkBehaviour
{
    private CharacterController _cc;

    private float maxSpeed = 2f;

    private bool _jumpPressed;
    public float JumpForce = 5f;
    public float GravityValue = -9.8f;

    public float rotationSpeed = 50.0f;

    private Vector3 _velocity;

    private Camera camera;

    private Animator _animator;
    
    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            _jumpPressed = true;
    }

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
        _animator = GetComponentInChildren<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
            return;

        if (_cc.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        var cameraRotationY = Quaternion.Euler(0, camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(horizontal, 0, vertical) * Runner.DeltaTime *
                       maxSpeed;

        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _cc.isGrounded)
            _velocity.y += JumpForce;

        Vector3 resultMove = move + _velocity * Runner.DeltaTime;
        _cc.Move(resultMove);

        if (move != Vector3.zero)
        {
            Vector3 forwardVelocity = vertical * camera.transform.forward;
            Vector3 horizontalVelocity = horizontal * camera.transform.right;

            // Lerp rotate the character.
            Vector3 characterDir = forwardVelocity + horizontalVelocity;

            Quaternion targetRotation = Quaternion.LookRotation(characterDir, Vector3.up);
            Quaternion resultRotation =
                Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);

            // Keep the character always up.
            resultRotation.x = 0;
            resultRotation.z = 0;
            transform.rotation = resultRotation;
        }

        float speed = _cc.velocity.magnitude;
        if (speed < Mathf.Epsilon)
        {
            _animator.SetBool(_isIdleHash, true);
        }
        else
        {
            _animator.SetBool(_isIdleHash, false);
        }
        
        _animator.SetFloat(_speed, speed);

        _jumpPressed = false;
    }
}