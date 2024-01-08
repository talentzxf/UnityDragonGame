using System;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class MissleMove : NetworkBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float initSpeed = 10.0f;

        [SerializeField] private float rotationSpeed = 100.0f;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        private Rigidbody rb;

        public override void Spawned()
        {
            base.Spawned();
            rb = GetComponent<Rigidbody>();
        }

        void AdjustAim(float delta)
        {
            if (Target == null)
                return;

            Vector3 heading = Target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(heading);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * delta));

            rb.velocity = transform.forward * initSpeed;
        }

       public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                AdjustAim(Runner.DeltaTime);
            }
        }
    }
}