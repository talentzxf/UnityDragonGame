using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private float rotationSpeed = 10.0f;

    private Animator _animator;
    private Transform _camera;

    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");
    private Transform _mainCameraTransform;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _mainCameraTransform = Camera.main.transform;

        if (_animator == null)
        {
            Debug.LogError("Can't find animator!!!");
        }
    }


    void Update()
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