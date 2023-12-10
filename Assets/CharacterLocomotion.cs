using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 5f;

    private Rigidbody _rigidbody;
    private Animator _animator;

    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Can't find animator!!!");
        }

        _animator.applyRootMotion = false;
    }

    
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 forwardVelocity = vertical * maxVelocity * transform.forward;
        Vector3 horizontalVelocity = horizontal * maxVelocity * transform.right;

        Vector3 resultVelocity = forwardVelocity + horizontalVelocity;
        _rigidbody.MovePosition(_rigidbody.position + resultVelocity * Time.deltaTime);

        float speed = resultVelocity.magnitude;
        Debug.Log("Speed:" + speed);
        if ( speed < Mathf.Epsilon)
        {
            _animator.SetBool(_isIdleHash, true);
            
            Debug.Log("IsIdle: true");
        }
        else
        {
            _animator.SetBool(_isIdleHash, false);
            Debug.Log("IsIdle: false");
        }
        
        _animator.SetFloat(_speed, speed);
    }
}
