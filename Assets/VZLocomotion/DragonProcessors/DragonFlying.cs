using UnityEngine;

namespace VZLocomotion.DragonProcessors
{
    public class DragonFlying : AnimatedCharactorLocomotionProcessor
    {
        private Rigidbody _rigidbody;

        private float maxSpeed = 5.0f;
        private float rotationVelocity = 100f;
        
        private int speedFWD = Animator.StringToHash("SpeedFWD");
        
        public override void OnActive(params object[] parameters)
        {
            base.OnActive(parameters);
            _rigidbody = _go.GetComponent<Rigidbody>();
            _rigidbody.useGravity = false; // Don't use gravity when flying.
        }

        public override void Update()
        {
            float vertical = Input.GetAxis("Vertical");
            Vector3 forwardVelocity = vertical * Time.deltaTime * maxSpeed * _transform.forward;
            _rigidbody.AddForce(forwardVelocity, ForceMode.VelocityChange);

            float horizontal = Input.GetAxis("Horizontal");
            if (horizontal != 0)
            {
                float rotationAmount = rotationVelocity * horizontal * Time.deltaTime;
                Quaternion deltaRotation = Quaternion.Euler(_transform.up * rotationAmount);
                _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);

                Vector3 velocity = _rigidbody.velocity;
                velocity = Quaternion.Euler(rotationAmount * Time.deltaTime * _transform.up) * velocity;
                _rigidbody.velocity = velocity.normalized * maxSpeed;
            }
            
            _animator.SetFloat(speedFWD, _rigidbody.velocity.magnitude);
        }
    }
}