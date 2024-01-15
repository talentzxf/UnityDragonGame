using System;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace DragonGameNetworkProject.FightSystem
{
    public class MissleMove : NetworkBehaviour
    {
        [SerializeField] private GameObject _explodePrefab;

        [SerializeField] private Transform _target;
        [SerializeField] private float initSpeed = 10.0f;

        [SerializeField] private float rotationSpeed = 100.0f;

        [SerializeField] private bool equipped = false;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        private Rigidbody rb;

        private Collider _collider;

        public override void Spawned()
        {
            base.Spawned();
            rb = GetComponent<Rigidbody>();

            equipped = false;
            _collider = GetComponent<Collider>();

            // _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && HasStateAuthority)
            {
                var inventory = other.GetComponentInParent<Inventory>();
                inventory.Equip(WEAPONTYPE.ROCKET);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                var otherColliderNO = contact.otherCollider.GetComponentInParent<NetworkObject>();
                var enemy = contact.otherCollider.GetComponentInParent<Enemy>();

                if (enemy is PlayerEnemy && otherColliderNO.HasInputAuthority) // Can't hit myself.
                    continue;

                {
                    var explosionPrefab = Instantiate(_explodePrefab);
                    explosionPrefab.transform.position = contact.point;
                    explosionPrefab.transform.rotation = Quaternion.LookRotation(contact.normal);

                    if (HasStateAuthority)
                    {
                        Runner.Despawn(GetComponent<NetworkObject>()); // Only State auth can despawn the rocket.
                        if (enemy)
                            enemy.DoDamageRpc(1000.0f);
                    }
                }
            }
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, Target.transform.position);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + rb.velocity * 10.0f);
            }
        }
#endif

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                AdjustAim(Runner.DeltaTime);
            }
        }
    }
}