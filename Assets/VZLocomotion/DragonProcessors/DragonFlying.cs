using UnityEngine;

namespace VZLocomotion.DragonProcessors
{
    public class DragonFlying : AnimatedCharactorLocomotionProcessor
    {
        private Rigidbody _rigidbody;

        private float maxSpeed = 500.0f;
        private float rotationSpeed = 1000f;

        private int speedFWD = Animator.StringToHash("SpeedFWD");

        public override void OnActive(params object[] parameters)
        {
            base.OnActive(parameters);
            _rigidbody = _go.GetComponent<Rigidbody>();
            _rigidbody.useGravity = false; // Don't use gravity when flying.
        }

        public override void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 force = (vertical * _transform.forward + horizontal * _transform.right).normalized *
                            Time.deltaTime * maxSpeed;

            _rigidbody.AddForce(force, ForceMode.Acceleration);

            Vector3 targetDirection = _rigidbody.velocity.normalized;

            Vector3 currentDirection = _transform.forward;
            Vector3 newDirection = Vector3.RotateTowards(currentDirection, targetDirection,
                rotationSpeed * Time.fixedDeltaTime, 0.0f);
            Quaternion newRotation = Quaternion.LookRotation(newDirection);

            _rigidbody.MoveRotation(newRotation);

            _animator.SetFloat(speedFWD, _rigidbody.velocity.magnitude);
        }
    }
}