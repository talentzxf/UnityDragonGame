using Fusion;
using UnityEngine;

public class PlayerMovementNetwork : NetworkBehaviour
{
    private CharacterController _cc;

    public float speed = 2f;

    private bool _jumpPressed;
    public float JumpForce = 5f;
    public float GravityValue = -9.8f;

    public float rotationSpeed = 50.0f;

    private Vector3 _velocity;

    public Camera camera;

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
                       speed;

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

        _jumpPressed = false;
    }
}