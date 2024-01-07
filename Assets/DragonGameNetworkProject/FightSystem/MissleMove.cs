using System;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class MissleMove: NetworkBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float initSpeed = 100.0f;

        [SerializeField] private float rotationSpeed = 100.0f;
        
        public Transform target => _target;

        private Rigidbody rb;

        public override void Spawned()
        {
            base.Spawned();
            rb = GetComponent<Rigidbody>();
        }

        void AdjustAim(float delta)
        {
            if (target == null)
                return;

            Vector3 heading = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(heading, transform.up);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * delta));
            
            rb.velocity = transform.up * initSpeed;
        }

        private bool hasLaunched = false; 
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                rb.velocity = transform.up * initSpeed;
                hasLaunched = true;
            }
        }

        private void FixedUpdate()
        {
            if (!hasLaunched)
                return;

            if (HasStateAuthority)
            {
                AdjustAim(Time.deltaTime);
            }            
        }
    }
}