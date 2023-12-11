using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private float rotationSpeed = 10.0f;
    private float maxVelocity = 1f;

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

        Vector3 forwardVelocity = vertical * maxVelocity * transform.forward;
        Vector3 horizontalVelocity = horizontal * maxVelocity * transform.right;

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
            Quaternion targetRotation = Quaternion.LookRotation(_mainCameraTransform.forward, Vector3.up);
            Quaternion resultRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            resultRotation.x = 0;
            resultRotation.z = 0;
            transform.rotation = resultRotation;
        }

        _animator.SetFloat(_speed, speed);
    }
}