using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 5f;

    private Animator _animator;
    private Transform _camera;

    private int _isIdleHash = Animator.StringToHash("isIdle");
    private int _speed = Animator.StringToHash("speed");

    void Start()
    {
        _animator = GetComponent<Animator>();

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
        
        if ( speed < Mathf.Epsilon)
        {
            _animator.SetBool(_isIdleHash, true);
        }
        else
        {
            // Lerp rotate the character.
            
            
            _animator.SetBool(_isIdleHash, false);
        }
        
        _animator.SetFloat(_speed, speed);
    }
}
