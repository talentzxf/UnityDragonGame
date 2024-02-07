using UnityEngine;

public class OnGroundProcessor : AnimatedCharactorLocomotionProcessor
{
    private float rotationSpeed = 10.0f;
    private Transform _mainCameraTransform;

    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");

    public override void Setup(CharacterLocomotion locomotion)
    {
        base.Setup(locomotion);

        _mainCameraTransform = Camera.main.transform;
    }

    public override void Update()
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
                Quaternion.Slerp(_transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Keep the character always up.
            resultRotation.x = 0;
            resultRotation.z = 0;
            _transform.rotation = resultRotation;
        }

        _animator.SetFloat(_speed, speed);
    }
}