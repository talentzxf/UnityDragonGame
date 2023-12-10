using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 5f;

    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        float maxVelocityChange = Time.deltaTime * maxVelocity;
        
        Vector3 forwardVelocity = vertical * maxVelocityChange * transform.forward;
        Vector3 horizontalVelocity = horizontal * maxVelocityChange * transform.right;

        _rigidbody.MovePosition(_rigidbody.position + forwardVelocity + horizontalVelocity);
    }
}
